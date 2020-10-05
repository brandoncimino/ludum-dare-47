using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace {
    public class MenuManager : MonoBehaviour {
        public enum MenuState {
            Main,
            HowToPlay
        }

        public GameObject playButton;
        public GameObject instructionButton;
        public GameObject backButton;
        public GameObject titleCard;
        public GameObject instructionBox;

        public MenuState currentState = MenuState.Main;

        public void ToInstructions() {
            instructionButton.SetActive(false);
            backButton.SetActive(true);
            titleCard.SetActive(false);
            instructionBox.SetActive(true);
        }

        //Doesn't need to be its own thing BUT WERE RUNNING OUT OF TIME AAAAHHHHHH!!!
        public void ToMenu() {
            instructionButton.SetActive(true);
            backButton.SetActive(false);
            titleCard.SetActive(true);
            instructionBox.SetActive(false);
        }

        public void StartGame() {
            SceneManager.LoadScene("Alpha1Main");
        }

    }
}