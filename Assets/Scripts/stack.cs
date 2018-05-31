using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class stack : MonoBehaviour {


	public GameObject particleSystem;
	public Text score;
	public GameObject endPanel;
	public Color32[] gameColor = new Color32[4];
	public Material stackMate;
	private const float BOUND_SIZE = 3.5f;
	private const float STACK_MOVING_SPEED = 6f;
	private const float ERROR_MARGIN = 0.1f;

	private int stackIndex;
	private int scoreCount;
	private GameObject[] theStack;

	private float tileTranstion = 0.0f;
	private float tileSpeed = 2.5f;
	private bool isMovingOnX = true;
	private bool gameOver=false;

	private int combo = 0;
	private float secondaryPosition;

	private Vector3 lastTilePosition;
	private Vector3 desiredPosition;
	private Vector2 stackBounds = new Vector2 (BOUND_SIZE, BOUND_SIZE);
	// Use this for initialization
	void Start () {
		theStack=new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			theStack [i] = transform.GetChild (i).gameObject;
			ColorMesh (theStack [i].GetComponent<MeshFilter> ().mesh);
		}

		stackIndex = transform.childCount - 1;

	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver)
			return;
		
		if (Input.GetMouseButtonDown (0)) {
			//PlaceTile ();
			if (PlaceTile()) {
				SpawnTile ();
				scoreCount++;
				score.text = scoreCount.ToString ();
			} else {
				EndGame ();
			}
		}

		MoveTile ();

		transform.position = Vector3.Lerp (transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);

	}

	private void MoveTile()
	{
		
		tileTranstion += Time.deltaTime * tileSpeed;
		if (isMovingOnX)
			theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTranstion) * BOUND_SIZE, scoreCount, secondaryPosition);
		else
			theStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTranstion) * BOUND_SIZE);	
	}

	private void SpawnTile()
	{
		lastTilePosition = theStack [stackIndex].transform.localPosition;

		stackIndex--;
		if (stackIndex < 0)
			stackIndex = transform.childCount - 1;

		desiredPosition = (Vector3.down) * scoreCount;
		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		theStack [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
		ColorMesh (theStack [stackIndex].GetComponent<MeshFilter> ().mesh);

	}
	private bool PlaceTile()
	{
		Transform t = theStack [stackIndex].transform;
		if (isMovingOnX) {
			float deltaX = lastTilePosition.x - t.position.x;
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) {
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				if (stackBounds.x < 0)
					return false;

				float middle = (lastTilePosition.x + t.localPosition.x) / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

				createRubble (
					new Vector3((t.position.x>0)
						? t.position.x + (t.localScale.x/2)
						:t.position.x - (t.localScale.x/2),
						t.position.y,
						t.position.z),
					new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
				);
				t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
			} else 
			{


				combo++;
				if (combo > 3) {
					stackBounds.x += 0.25f;
					if (stackBounds.x > BOUND_SIZE)
						stackBounds.x = BOUND_SIZE;

					float middle = (lastTilePosition.x + t.localPosition.x) / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

				}
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
				this.transform.GetComponent<AudioSource> ().Play ();
				GameObject pts = Instantiate (particleSystem)as GameObject;
				pts.transform.position = t.localPosition;
				Destroy (pts, 1f);
			}

		} else {
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y < 0)
					return false;

				float middle = (lastTilePosition.z + t.localPosition.z) / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				createRubble (
					new Vector3(t.position.x,
						t.position.y,
					    (t.position.z>0)
						? t.position.z + (t.localScale.z/2)
						:t.position.z - (t.localScale.z/2)),
					new Vector3(t.localScale.x,1,Mathf.Abs(deltaZ))
				);
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
			
			} else {
				combo++;
				if (combo > 3) {
					stackBounds.y += 0.25f;
					if (stackBounds.y > BOUND_SIZE)
						stackBounds.y = BOUND_SIZE;
					float middle = (lastTilePosition.z + t.localPosition.z) / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
				}
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
				this.transform.GetComponent<AudioSource> ().Play ();
				GameObject pts = Instantiate (particleSystem)as GameObject;
				pts.transform.position = t.localPosition;
				Destroy (pts, 1f);
			}
		}



		secondaryPosition = (isMovingOnX) ? t.localPosition.x : t.localPosition.z;
		isMovingOnX = !isMovingOnX;
		return true;

	}

	private void EndGame()
	{

		AdManager.ShowAd ();
		if (PlayerPrefs.GetInt ("score") < scoreCount)
			PlayerPrefs.SetInt ("score", scoreCount);
		gameOver = true;
		theStack [stackIndex].AddComponent<Rigidbody> ();
		endPanel.SetActive (true);
	}

	private void createRubble(Vector3 pos,Vector3 scale)
	{
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();
		go.GetComponent<MeshRenderer> ().material = stackMate;
		ColorMesh (go.GetComponent<MeshFilter> ().mesh);
		Destroy(go,1.75f);
	}

	private Color32 Lerp4(Color32 a,Color32 b,Color32 c,Color32 d,float t)
	{
		if (t < 0.33f)
			return Color.Lerp (a, b, t / 0.33f);
		else if (t < 0.66f)
			return Color.Lerp (b, c, (t-0.33f) / 0.33f);
		else
			return Color.Lerp (c, d, (t-0.66f) / 0.66f);
	}

	private void ColorMesh(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount * 0.25f);
		for (int i = 0; i < vertices.Length; i++)
			colors [i] = Lerp4 (gameColor [0], gameColor [1], gameColor [2], gameColor [3], f);
		mesh.colors32 = colors;
		

	}

	public void OnButtonClick(string sceneName)
	{
		if (sceneName == "game")
			SceneManager.LoadScene (1);
		else if (sceneName == "menu")
			SceneManager.LoadScene (0);
	}


}
