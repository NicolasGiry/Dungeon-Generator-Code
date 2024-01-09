using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    Tile[] tiles;
    public int rows, columns;
    public GameObject tilePref;
    public int iterationMax;

    public Slider sliderIt;
    public TextMeshProUGUI itText;
    void Awake()
    {
        sliderIt.onValueChanged.AddListener((v) =>
        {
            itText.text = v.ToString("00");
            iterationMax = (int) v;
        });

        tiles = new Tile[rows * columns];
        Init(1);
    }

    public void Init(int firstCall = 0)
    {
        int cnt = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (firstCall == 1)
                {
                    tiles[cnt] = Instantiate(tilePref, new Vector3(i + 0.5f, 0.5f, j + 0.5f), Quaternion.identity).GetComponent<Tile>();
                }
                int e = Random.Range(0, 2);
                tiles[cnt].etat = e == 0;
                cnt++;
            }
        }

        CreateMap();
    }

    public void CreateMap()
    {
        for (int i = 0; i < iterationMax; i++)
        {
            foreach (Tile tile in tiles)
            {
                tile.TrouverEtatSuivant();
            }
            foreach (Tile tile in tiles)
            {
                tile.ChangerEtat();
            }
        }
    }
}
