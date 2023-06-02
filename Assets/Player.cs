using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    // налаштування камери
    public Vector3 startPos;
    public float newX;
    public float newY;
    public float speed;
    public float speed2;
    public float pos;
    public float streak;

    // герой
    public Animator Stickman;
    public Animation Idle;
    public Animation Jumping;
    public GameObject lostPlayer;
    public GameObject thisGo;
    public GameObject thisGoBody;


    // локація
    public List<GameObject> platforms;
    public int currentPlatform;
    public List<float> positions;
    public List<GameObject> yellowCubes;
    public bool movingCubes;
    public List<GameObject> redWalls;
    public int currentRedWalls;
    public GameObject currentWaal;

    public float maximumCubes;
    public float countYellow; // підрахунок кубів під героєм

    //ui
    public GameObject textTaptoPlay;
    public GameObject gameOver;

    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    public void Start()
    {
        // розставляє жовті куби. на кожній платформі по 3, рандом позиції куба по осі Х
        foreach (GameObject platform in platforms)
        {

            platform.transform.GetChild(1).transform.localPosition = new Vector3(positions[Random.Range(0, 4)], 0.1f, -0.3f);
            platform.transform.GetChild(2).transform.localPosition = new Vector3(positions[Random.Range(0, 4)], 0.1f, -0.5f);
            platform.transform.GetChild(3).transform.localPosition = new Vector3(positions[Random.Range(0, 4)], 0.1f, -0.7f);

            foreach (Transform child in platform.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        // розставляє червоні куби, легкі на початку
        redWalls[0].SetActive(true);
        redWalls[3].SetActive(true);
        redWalls[Random.Range(6, 8)].SetActive(true);

        movingCubes = true;
    }

    public void Update()
    {
        // прив'язує спрайт "програвшого" до основного
        lostPlayer.transform.position = new Vector3(newX, newY, thisGo.transform.position.z);
        // рухає персонажа до осей
        thisGo.transform.position = Vector3.MoveTowards(thisGo.transform.position, new Vector3(thisGo.transform.position.x, newY, thisGo.transform.position.z), speed2 * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(newX, transform.position.y, transform.position.z), speed2 * Time.deltaTime);
        // рухає куби за персонажем
        foreach (GameObject go in platforms)
        {
            go.transform.position = Vector3.MoveTowards(go.transform.position, new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z + speed), speed * Time.deltaTime);
        }


        // якщо утримання
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    speed = 8; // швидкість руху платформ
                    startPos = touch.position;
                    textTaptoPlay.SetActive(false);
                    if(gameOver.activeSelf == true)
                    {
                        Reload();
                    }
                    break;
                case TouchPhase.Moved:
                    pos = touch.position.x;
                    if (movingCubes)
                    {
                        // рух пальця
                        if (pos - startPos.x < 50 && pos - startPos.x > -50)
                        {
                            newX = 0;
                        }
                        else if (pos - startPos.x > 50 && pos - startPos.x < 250) 
                        {

                            newX = -1f;
                            // тут же оновлюється старт поз
                        }
                        else if (pos - startPos.x > 250)
                        {
                            newX = -2f;
                        }
                        else if (pos - startPos.x < -50 && pos - startPos.x > -250)
                        {
                            newX = 1f;
                        }
                        else if (pos - startPos.x < -250)
                        {
                            newX = 2f;
                        }
                    }

                    break;
                case TouchPhase.Ended:
                  //  speed = 0;
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // якщо віддалились від пройденої стіни, вона зникає
        if (other.CompareTag("redWall"))
        {
            movingCubes = true;
            foreach (Transform child in currentWaal.transform)
            {
                child.gameObject.SetActive(false);
            }



            // жовті куби, що відділились від героя, повертаються
            foreach (GameObject cube in yellowCubes)
            {
                cube.transform.SetParent(thisGo.transform);
            }
            posCheck(); // оновлення позицій гравця і кубів
        }
        // коли пройшли платформу
        else if (other.CompareTag("Track"))
        {
            if (currentPlatform != -1)
            {
                // ставить пройдену платформу за останньою для нескіненності циклу
                if (currentPlatform - 1 >= 0)
                {
                    platforms[currentPlatform].transform.localPosition = new Vector3(0, transform.localPosition.y, platforms[currentPlatform - 1].transform.localPosition.z - 30f);
                }
                else
                {
                    platforms[currentPlatform].transform.localPosition = new Vector3(0, transform.localPosition.y, platforms[4].transform.localPosition.z - 30f);
                }
                // на ній генерується червоне й жовте
                platforms[currentPlatform].transform.GetChild(1).transform.localPosition = new Vector3(positions[Random.Range(0, 4)], 0.1f, -0.3f);
                platforms[currentPlatform].transform.GetChild(2).transform.localPosition = new Vector3(positions[Random.Range(0, 4)], 0.1f, -0.5f);
                platforms[currentPlatform].transform.GetChild(3).transform.localPosition = new Vector3(positions[Random.Range(0, 4)], 0.1f, -0.7f);
                platforms[currentPlatform].transform.GetChild(1).gameObject.SetActive(true);
                platforms[currentPlatform].transform.GetChild(2).gameObject.SetActive(true);
                platforms[currentPlatform].transform.GetChild(3).gameObject.SetActive(true);


            }
            if (currentPlatform < 4)
            {
                currentPlatform += 1;
            }
            else
            {
                currentPlatform = 0;
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("yellowCubes")) // об жовте
        {
            other.gameObject.SetActive(false);
            // створює куб під гравцем, якщо є місце
            foreach (GameObject cube in yellowCubes)
            {
                cube.transform.localPosition = new Vector3(0, cube.transform.localPosition.y, 0);
            }
            if (countYellow != maximumCubes)
            {
                countYellow += 1;
                posCheck();
                Stickman.Play("Jumping");
            }
        }
       else  if(other.CompareTag("redWall"))
            {
            movingCubes = false; 
            // генерує нові стіни
            currentWaal = other.gameObject;
            redWalls[Random.Range(currentRedWalls - 1, currentRedWalls + 1)].SetActive(true);
            if (currentRedWalls < 13)
            {
                currentRedWalls += 3;
            }
            else
            {
                currentRedWalls = 1;
            }
        }
    }

    public void posCheck()
    {
        if (countYellow <= 0)
        {
            gameOver.SetActive(true);
            speed = 0;
            thisGoBody.SetActive(false);
            lostPlayer.SetActive(true);
        }
        else
        {
            newY = 0.9f * (countYellow - 1);
        }
        // стак кубів
        if (movingCubes)
        {
            yellowCubes[0].transform.localPosition = new Vector3(0, -0.45f, 0);
            if (countYellow > 1)
            {
                foreach (GameObject cube in yellowCubes)
                {
                    cube.SetActive(false);
                }
                yellowCubes[0].SetActive(true);
                for (int i = 1; i < countYellow - 1; i++)
                {
                    yellowCubes[i].transform.localPosition = new Vector3(0, yellowCubes[i - 1].transform.localPosition.y - 0.9f, 0);
                    yellowCubes[i].SetActive(true);
                }
            }
            
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
