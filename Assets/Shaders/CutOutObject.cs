using System.Collections.Generic;
using UnityEngine;

public class CutOutObject : EaseControllerBase
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallMask;

    [SerializeField] private float detectionRadius;
    [SerializeField] private float cutSize;

    [SerializeField] private float fadeDurtation = 1f;
    [SerializeField] private EasingUtility.Style easingStyle;
    private float fade;
    
    private Vector3 direction;
    private Camera camera; 
    
    private static readonly int CutPos = Shader.PropertyToID("_CutPos");
    private static readonly int Size = Shader.PropertyToID("_CutSize");
    private static readonly int OffSize = Shader.PropertyToID("_FallOffSize");
    
    //private Material material;
    private HashSet<Material> buffer = new HashSet<Material>();
    
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        foreach (var element in buffer)
        {
            SetReverse(false);
            Play();
            element.SetFloat(OffSize, fade);
        }
        buffer.Clear();
        
        Vector3 cutOutPos = camera.WorldToViewportPoint(player.position);
        cutOutPos.y /= (Screen.width / Screen.height);

        direction = player.transform.position - camera.transform.position;
        RaycastHit[] hits = Physics.SphereCastAll(camera.transform.position, 
            detectionRadius, direction.normalized, direction.magnitude, 
            wallMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Material[] materials = hits[i].transform.
                GetComponent<Renderer>().materials;

            foreach (var material in materials)
            {
                buffer.Add(material);
            }
        }
        
        foreach (var element in buffer)
        {
            float holeSize = cutSize / direction.magnitude;
            element.SetVector(CutPos, cutOutPos);
            element.SetFloat(Size, holeSize);
            
            SetReverse(true);
            Play();
            element.SetFloat(OffSize, fade);
        }
    }

    public override void OnStart()
    {
        SetDuration(fadeDurtation);
        SetStyle(easingStyle);
    }

    public override void Evaluate(float t)
    {
       fade = EasingUtility.Interpolate(0f, 0.1f, t);
    }

    public override void OnEnd()
    {
        
    }
}
