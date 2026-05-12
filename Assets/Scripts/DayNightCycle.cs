using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Nastavení")]
    public Light2D globalLight; // Přetáhni sem své Global Light 2D
    public Gradient denniCyklus; // Tady si v Inspectoru naklikáš barvy
    public float delkaDne = 60f;

    
    public float aktualniCas = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        aktualniCas += Time.deltaTime / delkaDne;

        // Pokud přetečeme přes 1, vrátíme se na 0 (opakování)
        if (aktualniCas >= 1f) aktualniCas = 0f;

        // Nastavíme barvu světla podle gradientu v daném čase
        globalLight.color = denniCyklus.Evaluate(aktualniCas);
    }
}
