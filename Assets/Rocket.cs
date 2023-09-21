using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip Success;
    [SerializeField] AudioClip Death;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem SuccessParticle;
    [SerializeField] ParticleSystem DeathParticle;

    Rigidbody rigidBody;
	AudioSource audioSource;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    bool collisionsEnable = false;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		ProcessInput();
	}

    private void ProcessInput()
    {
        if(state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            // todo only if debug on
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // toggle collision
            collisionsEnable = !collisionsEnable;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collisionsEnable)
        {
            return;
        }

        switch(collision.gameObject.tag)
        {
            case "Start":
                {
                    print("OK");
                    break;
                }
            case "End":
                {
                    StartSuccess();
                    break;
                }
            default:
                {
                    StartDeath();
                    break;
                }
        }
    }

    private void StartDeath()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        mainEngineParticle.Stop();
        DeathParticle.Play();
        Invoke("LoadFirstLvel", levelLoadDelay);
    }

    private void StartSuccess()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(Success);
        mainEngineParticle.Stop();
        SuccessParticle.Play();
        Invoke("LoadNextLevel", levelLoadDelay); // first one is call the func, second one is parameterise time
    }

    private void LoadFirstLvel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        print(currentSceneIndex);
        int nextSceneIndex = currentSceneIndex + 1;
        print(SceneManager.sceneCountInBuildSettings);
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        print(nextSceneIndex);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void Rotate()
    {
        // take manual control of rotation
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            mainEngineParticle.Play();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }
}
