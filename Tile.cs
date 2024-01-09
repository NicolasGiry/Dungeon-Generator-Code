using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool etat;
    bool etatSuivant;
    MeshRenderer meshRenderer;
    int cnt;
    void Awake()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void TrouverEtatSuivant()
    {
        cnt = 0;

        CheckVoisins(new Vector3(1.0f, 0.0f, 0.0f));
        CheckVoisins(new Vector3(1, 0, 1));
        CheckVoisins(new Vector3(1, 0, -1));
        CheckVoisins(new Vector3(0, 0, 1));
        CheckVoisins(new Vector3(0, 0, -1));
        CheckVoisins(new Vector3(-1, 0, 0));
        CheckVoisins(new Vector3(-1, 0, 1));
        CheckVoisins(new Vector3(-1, 0, -1));

        if (cnt == 4)
        {
            // égalité : on détermine aléatoirement
            int e = Random.Range(0, 2);
            etatSuivant = e == 0;
        }
        else
        {
            etatSuivant = cnt > 4;
        }
    }

    void CheckVoisins(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 100f))
        {
            if (hit.collider.CompareTag("Tile") && hit.collider.gameObject.GetComponent<Tile>().etat)
            {
                cnt++;
            }
        } 
    }

    public void ChangerEtat()
    {
        etat = etatSuivant;
        meshRenderer.enabled = etat;
        gameObject.GetComponent<BoxCollider>().isTrigger = !etat;
    }
}
