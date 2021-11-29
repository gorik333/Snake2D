using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    [SerializeField]
    private GameObject _particleSystem;


    public void SpawnParticles()
    {
        GameObject particles = Instantiate(_particleSystem, transform.position, Quaternion.identity);

        Destroy(particles, 2f);
    }


    public void SpawnParticles(Color color)
    {
        GameObject particles = Instantiate(_particleSystem, transform.position, Quaternion.identity);

        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();

        var renderer = particleSystem.GetComponent<Renderer>();

        renderer.material.color = color;

        Destroy(particles, 2f);
    }
}
