using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Enable Form Navigation up and down, and submit button with keyboard.
/// Requiered: In the Event System created with the Canvas 
/// assign a "First Selected" object
/// </summary>
public class FormNavigation : MonoBehaviour
{
    private EventSystem system;

    [SerializeField] private Button submitButton;

    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;

        if (!system)
        {
            Debug.LogError(
                this + ": Event System required");
            return;
        }
        else if (!system.firstSelectedGameObject)
            Debug.LogError(
                this + ": Event System First Selected Object is required");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable previous = 
                system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (previous != null)
            {
                previous.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = 
                system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
    }
}