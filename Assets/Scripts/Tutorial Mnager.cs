using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text TutorialText;
    public GameObject[] popUps;
    private int popUpIndex;
    public float waitTime = 10f;

    public GameObject GV;
    public GameObject PV;
    public GameObject SV;
    public GameObject RV;
    public GameObject OV;
    public GameObject BV;

    public GameObject character;
    public GameObject character2;

    public GameObject tut1;
    public GameObject tut2;

    private CombatBase Attack;
    private PlayerEvasiveRoll Evade;
    private PlayerSlipStep Step;

    private CombatBase Attack2;
    private PlayerEvasiveRoll Evade2;
    private PlayerSlipStep Step2;

    public HealthBar healthBar;
    public HealthBar healthBar2;

    public AdrenalineBar adbar;
    public AdrenalineBar adbar2;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            Attack = character.GetComponent<CombatBase>();
            Attack.enabled = false;
            Evade = character.GetComponent<PlayerEvasiveRoll>();
            Evade.enabled = false;
            Step = character.GetComponent<PlayerSlipStep>();
            Step.enabled = false;

            Attack2 = character.GetComponent<CombatBase>();
            Attack2.enabled = false;
            Evade2 = character.GetComponent<PlayerEvasiveRoll>();
            Evade2.enabled = false;
            Step2 = character.GetComponent<PlayerSlipStep>();
            Step2.enabled = false;

            PV.SetActive(false);
            GV.SetActive(false);
            SV.SetActive(false);
            RV.SetActive(false);
            OV.SetActive(false);
            BV.SetActive(false);
            tut1.SetActive(false);
            tut2.SetActive(false);
        }
        else
        {
            adbar.ActivateAdrenaline();
            adbar2.ActivateAdrenaline();
        }
    }

    private void Update()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[i].SetActive(true);
            }
            else
            {
                popUps[i].SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && popUpIndex != 15)
        {
            waitTime = 15;
            popUpIndex++;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Demo1");
        }

        if (healthBar.health < 80)
        {
            healthBar.health = healthBar.maxHealth;
        }

        if (healthBar2.health < 80)
        {
            healthBar2.health = healthBar2.maxHealth;
        }

        if (popUpIndex == 0)
        {
            if (waitTime <= 0)
            {
                waitTime = 5;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 1)
        {
            if (waitTime <= 1)
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A))
                {
                    waitTime = 5;
                    popUpIndex++;
                }
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 2)
        {
            Step.enabled = true;
            Step2.enabled = true;
            if (waitTime <= 0)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    waitTime = 5;
                    popUpIndex++;
                }
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 3)
        {
            Evade.enabled = true;
            Evade2.enabled = true;
            if (waitTime <= 0)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    waitTime = 10;
                    popUpIndex++;
                }
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 4)
        {
            tut1.SetActive(true);
            tut2.SetActive(true);
            Attack.enabled = true;
            Attack2.enabled = true;
            if (waitTime <= 0)
            {
                if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L))
                {
                    waitTime = 5;
                    popUpIndex++;
                }
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 5)
        {
            if (waitTime <= 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitTime = 15;
                    popUpIndex++;
                }
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }


        if (popUpIndex == 6)
        {
            BV.SetActive(true);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 7)
        {
            BV.SetActive(false);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 8)
        {
            PV.SetActive(true);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 9)
        {
            PV.SetActive(false);
            GV.SetActive(true);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 10)
        {
            GV.SetActive(false);
            SV.SetActive(true);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 11)
        {
            SV.SetActive(false);
            adbar.ActivateAdrenaline();
            adbar2.ActivateAdrenaline();
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 12)
        {
            RV.SetActive(true);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 13)
        {
            RV.SetActive(false);
            if (waitTime <= 0)
            {
                waitTime = 15;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 14)
        {
            OV.SetActive(true);
            if (waitTime <= 0)
            {
                waitTime = 10;
                popUpIndex++;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (popUpIndex == 15)
        {
            OV.SetActive(false);
            if (waitTime <= 0)
            {
                waitTime = 10;
                SceneManager.LoadScene("Demo1");
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}
