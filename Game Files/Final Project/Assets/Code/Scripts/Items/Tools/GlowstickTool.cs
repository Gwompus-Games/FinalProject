using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowstickTool : ToolsParent
{
    [SerializeField] private float _throwForce = 15;

    private Material _material;
    private Color _chosenColour;
    private II_Glowstick _myInventoryItem;
    private ToolController _toolController;

    public override void Init()
    {
        base.Init();
        GlowstickSO toolData = _myTool.toolData as GlowstickSO;
        if (toolData == null)
        {
            throw new System.Exception($"Wrong data type assigned for {gameObject}");
        }
        _material = GetComponentInChildren<MeshRenderer>().material;
        _toolController = GameManager.Instance.GetManagedComponent<ToolController>();
    }

    public void SetToolData(II_Glowstick item)
    {
        _myInventoryItem = item;
        AssignColour(_myInventoryItem.GetColour());
    }

    public void AssignColour(Color colourToAssign)
    {
        _chosenColour = colourToAssign;
        _material.color = colourToAssign;
    }

    public II_Glowstick GetToolInventoryItem()
    {
        return _myInventoryItem;
    }

    public override void UseTool()
    {
        if (_myInventoryItem == null)
        {
            Debug.LogWarning("Inventory item is null!");
            return;
        }
        if (_toolController.debugMode)
        {
            Debug.Log($"Using Glowstick: {_myInventoryItem.itemData.itemName} | That is colour: {_myInventoryItem.GetColour()}");
        }

        ThrowGlowstick(_myInventoryItem, _chosenColour);
        SetToolEnabled(false);
        _toolController.ResetTools();
    }

    public override void CancelUseTool()
    {
        
    }

    public void ThrowGlowstick(II_Glowstick glowstick, Color chosenColour)
    {
        if (_toolController.debugMode)
        {
            Debug.Log($"Throwing Glowstick: {glowstick.itemData.itemName} | That is colour: {chosenColour}");
        }

        //Add glowstick sound effect here

        Transform worldItemParent = FindFirstObjectByType<WorldItemsTag>().transform;
        WI_Glowstick glowstickWorldItem = Instantiate(glowstick.itemData.worldObject, worldItemParent, true).GetComponent<WI_Glowstick>();
        glowstickWorldItem.transform.position = transform.position;
        glowstickWorldItem.transform.rotation = transform.rotation;
        glowstickWorldItem.GetComponent<Rigidbody>().AddForce(transform.forward * (_throwForce * Random.Range(0.8f, 1.2f)), ForceMode.Impulse);
        glowstickWorldItem.GetComponent<Rigidbody>().AddTorque(transform.up * (_throwForce / 2 * Random.Range(0.8f, 1.2f)), ForceMode.Impulse);
        glowstickWorldItem.UseGlowstick(chosenColour);
        _toolController.RemoveTool(glowstick);
        Destroy(glowstick.gameObject);
    }
}
