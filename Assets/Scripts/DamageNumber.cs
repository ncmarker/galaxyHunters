using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public Text damageText;
    private float lifetime = 1f;
    private float floatSpeed = 1f;

    // Initialize via integer amount
    public void Initialize(int damageAmount, Color textColor)
    {
        damageText.text = damageAmount.ToString();
        damageText.color = textColor;
        StartCoroutine(FadeAndDestroy());
    }

    // initialize via string
    public void Initialize(string amount, Color textColor)
    {
        damageText.text = amount;
        damageText.color = textColor;
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Color startColor = damageText.color;

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPos + new Vector3(0, elapsedTime * floatSpeed, 0);

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifetime);
            damageText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}
