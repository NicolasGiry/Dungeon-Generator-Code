using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool etat;
    bool etatSuivant;
    MeshRenderer meshRenderer;
    int cnt;
    Tile[] voisins;
    void Awake()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        voisins = new Tile[8];
    }

    private void Start()
    {
        Vector3[] dir = { new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1, 0, 1), new Vector3(1, 0, -1), 
            new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(-1, 0, 1), new Vector3(-1, 0, -1) };

        for (int i = 0; i < 8; i++)
        {
            if (Physics.Raycast(transform.position, dir[i], out RaycastHit hit, 100f) && hit.collider.CompareTag("Tile"))
            {
                voisins[i] = hit.transform.gameObject.GetComponent<Tile>();
            }
        }
    }

    public void TrouverEtatSuivant()
    {
        cnt = 0;
        CheckVoisins();

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
    
    void CheckVoisins()
    {
        for (int i = 0; i < 8; i++)
        {
            if (voisins[i] != null && voisins[i].etat)
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
