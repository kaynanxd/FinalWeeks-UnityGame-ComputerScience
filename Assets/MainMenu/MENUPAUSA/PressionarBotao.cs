using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Button))] 
public class ButtonKeyListener : MonoBehaviour
{
    public KeyCode keyToPress = KeyCode.Escape; 

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            button.onClick.Invoke();
        }
    }
}