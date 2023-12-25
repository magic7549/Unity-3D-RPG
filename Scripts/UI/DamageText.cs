using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DamageText : MonoBehaviour
{

    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    TextMeshPro text;
    Color alpha;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        text=GetComponent<TextMeshPro>();
        text.text = damage.ToString();
        alpha = text.color;
        Invoke("DestroyObject", destroyTime);

        transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        transform.LookAt(Camera.main.transform);
        text.transform.localScale = new Vector3(-1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0,moveSpeed * Time.deltaTime,0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime*alphaSpeed);
        text.color = alpha;
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

}
