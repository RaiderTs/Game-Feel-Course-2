using UnityEngine;

public class DeathSplatterHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Health.OnDeath += SpawnDeathVFX;
        Health.OnDeath += SpawnDeathSplatterPrefab;
    }

    private void OnDisable()
    {
        Health.OnDeath -= SpawnDeathVFX;
        Health.OnDeath -= SpawnDeathSplatterPrefab;
    }

    private void SpawnDeathSplatterPrefab(Health sender) // паттерн обсервер
    {
        GameObject newSplatterPrefab =
            Instantiate(sender.SplatterPrefab, sender.transform.position, sender.transform.rotation);
        SpriteRenderer deathSplatterSpriteRenderer = newSplatterPrefab.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>(); // получаем компонент ColorChanger

        if (colorChanger)
        {
            Color currentColor = colorChanger.DefaultColor; // получаем цвет из компонента ColorChanger
            deathSplatterSpriteRenderer.color = currentColor;
        }

        newSplatterPrefab.transform.SetParent(this.transform); // назначаем его потомком родительского класса
    }

    private void SpawnDeathVFX(Health sender)
    {
        GameObject deathVFX = Instantiate(sender.DeathVFX, sender.transform.position, sender.transform.rotation);
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>(); // получаем компонент ColorChanger

        if (colorChanger)
        {
            Color currentColor = colorChanger.DefaultColor; // получаем цвет из компонента ColorChanger
            ps.startColor = currentColor;
        }


        deathVFX.transform.SetParent(this.transform);
    }
}