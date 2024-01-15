using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    Tile[] tiles;
    public int rows, columns;
    public GameObject tilePref;
    public int iterationMax;
    public int seed;
    bool personnalSeed;

    public Slider sliderIt;
    public TextMeshProUGUI itText;
    public TMP_InputField seedText;
    public Toggle personnalSeedCheck;
    void Awake()
    {
        sliderIt.onValueChanged.AddListener((v) =>
        {
            itText.text = v.ToString("00");
            iterationMax = (int) v;
        });

        personnalSeed = false;
        seedText.interactable = false;

        seedText.characterLimit = 9;

        tiles = new Tile[rows * columns];
        
    }

    private void Start()
    {
        Init(1);
    }

    private void Update()
    {
        personnalSeed = personnalSeedCheck.isOn;
        seedText.interactable = personnalSeed;
    }

    public void Init(int firstCall = 0)
    {
        GenerateSeed();
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

    private void GenerateSeed()
    {
        if (!personnalSeed)
        {
            seed = Random.Range(0, 999999999);
            Random.seed = seed;
            seedText.text = seed.ToString();
        }
        else
        {
            Random.seed = System.Convert.ToInt32(seedText.text);
        }
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
