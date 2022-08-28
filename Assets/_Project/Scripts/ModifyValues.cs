using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModifyValues : MonoBehaviour
{
    private float gravityScale = 0;
    private float pipeGap = 0;
    private float jumpHeight = 0;
    private float spawnTime = 0;
    private float moveSpeed = 0;
    private bool toggleCollision = false;

    public Action<string> OnValueChanged;

    [SerializeField] private TMP_Text gravityPlaceholder;
    [SerializeField] private TMP_Text pipePlaceholder;
    [SerializeField] private TMP_Text jumpPlaceHolder;


    [SerializeField] private GameObject spawnGO;
    [SerializeField] private GameObject speedGO;

    private TMP_InputField spawnText;
    private TMP_InputField speedText;

    [SerializeField] private Slider spawnSlider;
    [SerializeField] private Slider moveSlider;

    private void Start()
    {
        gravityPlaceholder.text = GravityScale.ToString();
        pipePlaceholder.text = PipeGap.ToString();
        jumpPlaceHolder.text = JumpHeight.ToString();

        spawnGO.TryGetComponent(out spawnText);
        speedGO.TryGetComponent(out speedText);

        spawnText.text = spawnSlider.value.ToString("F1");
        speedText.text = moveSlider.value.ToString("F1");
    }

    public float GravityScale 
    { 
        get => gravityScale; 
        set 
        {
            gravityScale = value;
            OnValueChanged?.Invoke(nameof(GravityScale));
        } 
    }

    public bool DisableCollisions
    {
        get => toggleCollision;
        set
        {
            toggleCollision = value;
            OnValueChanged?.Invoke(nameof(DisableCollisions));
        }
    }

    public float PipeGap
    {
        get => pipeGap;
        set
        {
            pipeGap = value;
            OnValueChanged?.Invoke(nameof(PipeGap));
        }
    }

    public float JumpHeight
    {
        get => jumpHeight;
        set
        {
            jumpHeight = value;
            OnValueChanged?.Invoke(nameof(JumpHeight));
        }
    }

    public float SpawnTime
    {
        get => spawnTime;
        set
        {
            spawnTime = value;
            spawnSlider.value = value;
            OnValueChanged?.Invoke(nameof(SpawnTime));
        }
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            moveSpeed = value;
            moveSlider.value = value;
            OnValueChanged?.Invoke(nameof(MoveSpeed));
        }
    }

    public void ReadGravityScale(string _gravityScale)
    {
        if (float.TryParse(_gravityScale, out float scale))
        {
            GravityScale = scale;
        }
    }

    public void ReadPipeGap(string _pipeGap)
    {
        if (float.TryParse(_pipeGap, out float gap))
        {
            PipeGap = gap;
        }
    }

    public void ReadJumpHeight(string _jumpHeight)
    {
        if (float.TryParse(_jumpHeight, out float height))
        {
            JumpHeight = height;
        }
    }

    public void ReadSpawnTime(string _spawnTime)
    {
        if (float.TryParse(_spawnTime, out float time))
        {
            SpawnTime = time;
        }
    }

    public void ReadMoveSpeed(string _moveSpeed)
    {
        if (float.TryParse(_moveSpeed, out float speed))
        {
            MoveSpeed = speed;
        }
    }

    public void OnChangeSpeed(float changedValue)
    {
        MoveSpeed = changedValue;
        if (speedText != null) speedText.text = moveSlider.value.ToString("F1");
    }

    public void OnChangeSpawn(float changedValue)
    {
        SpawnTime = changedValue;
        if (spawnText != null) spawnText.text = spawnSlider.value.ToString("F1");
    }
}
