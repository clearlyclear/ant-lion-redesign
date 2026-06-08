using UnityEngine;
using System.Collections;

public class AntBehaviour : MonoBehaviour {

	private GameObject grid;
	private GridMaker gridMaker;
	private GlobalState globalState;
	private GameObject cameraObject;
	private GameObject sand;
	private Bounds antBounds;
	private CellGrid gridData;

	Vector2 antPos;
	Vector2 targetPos;
	enum MoveType {NONE, HORIZONTAL, VERTICAL, LANDSLIDE};
	MoveType movementType = MoveType.NONE;
	float movementStartTime;
	public float movementDurationSeconds = 1.0f;

	private bool isMoving => movementType != MoveType.NONE;

	// Use this for initialization
	void Start () {
		//grab the grid maker
		grid = GameObject.Find ("Grid");
		DebugUtils.Assert(grid);
		gridMaker = grid.GetComponent<GridMaker> ();
		DebugUtils.Assert(gridMaker);

		sand = GameObject.Find ("SandBackground");
		DebugUtils.Assert (sand);

		cameraObject = GameObject.Find ("Main Camera");
		DebugUtils.Assert(cameraObject);

		//make sure we have a global state - create one if it's missing (probably started scene from within Unity editor)
		GameObject glob = GameObject.Find ("GlobalState");
		if (glob) {
			globalState = glob.GetComponent<GlobalState>();
		} else {
			globalState = grid.AddComponent<GlobalState>();
			Debug.LogWarning("Creating global state object on the fly - assuming game is running in editor");
		}

		globalState.globalState = GlobalState.GameState.PLAYING;

		//size the ant to fill one grid cell, and put it in a bottom centre cell
		Sprite sprite = GetComponent<SpriteRenderer> ().sprite;
		antBounds = sprite.bounds;
		transform.localScale = new Vector3(
   	 		1.0f/antBounds.size.x * gridMaker.gridCellWidth,
    		1.0f/antBounds.size.y * gridMaker.gridCellHeight,
    		1.0f
		);

		gridData = new CellGrid(gridMaker.gridSizeHorizontal, gridMaker.gridSizeVertical);

		antPos.x = gridMaker.gridSizeHorizontal / 2;
		antPos.y = 0;
		targetPos = antPos;

		InitializeMovement();
	}

	void InitializeMovement() {
		movementStartTime = Time.time;
	}

	void HandleInput() {
		if (isMoving) return;

		if (Input.GetKeyDown(KeyCode.UpArrow))
			MoveVertical(1);
		else if (Input.GetKeyDown(KeyCode.DownArrow))
			MoveVertical(-1);
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
			MoveHorizontal(-1);
		else if(Input.GetKeyDown(KeyCode.RightArrow))
			MoveHorizontal(1);
	}

	void FollowCamera() {
        if (cameraObject == null || sand == null) return;

        Camera cam = cameraObject.GetComponent<Camera>();
        Bounds sandBounds = sand.GetComponent<SpriteRenderer>().bounds;

        float camx = Mathf.Clamp(
            transform.position.x,
            sandBounds.min.x + cam.orthographicSize * cam.aspect,
            sandBounds.max.x - cam.orthographicSize * cam.aspect
        );

        float camy = Mathf.Clamp(
            transform.position.y,
            sandBounds.min.y + cam.orthographicSize,
            sandBounds.max.y - cam.orthographicSize
        );

        cameraObject.transform.position =
            new Vector3(camx, camy, cameraObject.transform.position.z);
    }

	Vector3 GridToWorld(Vector2 pos) {
		return grid.transform.position + new Vector3(
			pos.x * gridMaker.gridCellWidth,
			pos.y * gridMaker.gridCellHeight,
			0f
		);
	}

	void ResolveCell(int x, int y) {
		if (gridData == null) return;

		Cell cell = gridData.GetCell(x, y);
		if (cell == null) return;

		cell.revealed = true;

		if (cell.type == Cell.Type.Antlion) {
			GetComponent<SpriteRenderer>().color = Color.red;
			// TO DO: antlion animation trigger
			globalState.globalState = GlobalState.GameState.LOST;
			return;
		}

		if (cell.type == Cell.Type.Number) {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (cell.type == Cell.Type.Empty) {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
	}

	//update the ant location, and handle the current movement
	void UpdateMovement() {
		float elapsed = (Time.time-movementStartTime)/movementDurationSeconds;
		
		if (elapsed < 1f) {
			Vector3 start = GridToWorld(antPos);
			Vector3 target = GridToWorld(targetPos);
				
			float t = 1 - Mathf.Pow(Mathf.Cos(elapsed * Mathf.PI/2), 2);
			transform.position = Vector3.Lerp(start, target, t);

			return;
		}

		antPos = targetPos;

		if (antPos.y >= gridMaker.gridSizeVertical) {
			globalState.globalState = GlobalState.GameState.WON;
		}

		transform.position = GridToWorld(antPos);
		ResolveCell((int)antPos.x, (int)antPos.y);
		movementType = MoveType.NONE;
	}

	void MoveHorizontal(int steps) {
		Vector2 newPos = antPos + new Vector2(steps, 0);

		if (newPos.x < 0 || newPos.x >= gridMaker.gridSizeHorizontal) return;

		targetPos = newPos;
		movementType = MoveType.HORIZONTAL;
		InitializeMovement();
	}

	void MoveVertical(int steps, bool isLandSlide = false) {
		Vector2 newPos = antPos + new Vector2(0, steps);

		if (newPos.y < 0) {
			globalState.globalState = GlobalState.GameState.LOST;
		}

		targetPos = newPos;
		movementType = MoveType.VERTICAL;
		InitializeMovement();
	}

	// Update is called once per frame
	void Update () {
		HandleInput();
		UpdateMovement();
	}

	void LateUpdate() {
		FollowCamera();
	}
}
