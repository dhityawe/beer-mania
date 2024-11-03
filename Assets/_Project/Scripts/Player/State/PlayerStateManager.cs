using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PlayerStateManager : MonoBehaviour
{
    private IPlayerState currentState;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float switchTableDistance = 1.5f;

    [Header("Glass Pool")]
    public GlassPool glassPool;

    [Header("Pouring Settings")]
    public Image fillImage;
    public float fillRate = 0.3f;

    [Header("References")]
    public Transform[] tables;
    private int currentTableIndex = 0;

    public Animator anim;
    public float fillLevel = 0;

    public event Action<bool> MovingChanged; // Notify when moving
    public event Action Pouring; // Notify when pouring
    public event Action Throwing; // Notify when throwing

    private void Start()
    {
        anim = GetComponent<Animator>();
        SetState(new PlayerMoveState(this));
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void SetState(IPlayerState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    #region Input-Driven Actions

    public void MoveLeft()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    public void MoveRight()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }

    public void SwitchTable(int direction)
    {
        currentTableIndex += direction;

        if (currentTableIndex < 0)
        {
            currentTableIndex = tables.Length - 1; // Wrap to the last table
        }
        else if (currentTableIndex >= tables.Length)
        {
            currentTableIndex = 0; // Wrap to the first table
        }

        // Update the player's position to the new table's position
        transform.position = tables[currentTableIndex].position;
    }

    #endregion

    #region Pouring and Throwing Mechanics

    public void StartPouring()
    {
        fillLevel = 0;
        anim.SetTrigger("StartPouring");
        anim.SetBool("isPouring", true);
    }

    public void StopPouring()
    {
        anim.SetBool("isPouring", false);
    }

    public bool PourBeer()
    {
        // teleport the player to the table
        transform.position = tables[currentTableIndex].position;

        fillLevel += Time.deltaTime * fillRate;
        fillLevel = Mathf.Clamp(fillLevel, 0, 1);
        UpdateFillUI(fillLevel);

        return fillLevel >= 1;
    }
    public void ThrowGlass()
    {
        GameObject glass = glassPool.GetGlass();
        if (glass != null)
        {
            glass.transform.position = new Vector2(transform.position.x, tables[currentTableIndex].position.y);
            
            BeerQuality quality = DetermineQuality(fillLevel); // Use the enum
            glass.GetComponent<Glass>().SetQuality(quality); // Set quality in glass object
            glass.GetComponent<Glass>().SetTableIndex(currentTableIndex); // Set table index in glass object
            Debug.Log($"Glass thrown with fill level {fillLevel} ({quality}) quality.");
        }
    }

    private BeerQuality DetermineQuality(float fill) // Update to return BeerQuality
    {
        if (fill >= 0.65f && fill <= 0.75f) return BeerQuality.Perfect;
        if ((fill >= 0.55f && fill < 0.65f) || (fill > 0.75f && fill <= 0.85f)) return BeerQuality.Good;
        return BeerQuality.Bad;
    }

    private void UpdateFillUI(float fillLevel)
    {
        Debug.Log($"Fill Level: {fillLevel}");
    }

    #endregion
}
