using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCtrl : MonoBehaviour
{
    InputActions playerInput;
    InputAction menu;

    [SerializeField] GameObject pauseUI;
    [SerializeField] bool isPaused;

    void Awake(){
        playerInput = new InputActions();
    }

    void OnEnable(){
        menu = playerInput.Menu.Pause;
        menu.Enable();
        menu.performed += Pause;
    }

    void OnDisable(){
        menu.Disable();

    }

    void Pause(InputAction.CallbackContext context){
        isPaused = !isPaused;
        if (isPaused){
            ActivateMenu();
        }
        else{
            DeActivateMenu();
        }
    }

    public void ActivateMenu(){
        Time.timeScale = 0f;
        AudioListener.pause = true;
        pauseUI.SetActive(true);
    }

    public void DeActivateMenu(){
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseUI.SetActive(false);
        isPaused = false;
    }
    
}
