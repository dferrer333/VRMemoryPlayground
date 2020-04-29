using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class BlockPlacer : MonoBehaviour {
	
	Vector3 currentPotentialBlock;
	Vector3 prevPotentialBlock;
	GameObject highlightBlock;
	GameObject modelBlock;
	
	//Define the grid dimensions for blocks:
	Vector3 Grid = new Vector3(0.15f, 0.15f, 0.15f);
	
	//Toggle variable for highlight block:
	public bool blockHighlight = false;
 
    //Handles # of blocks the user has
    public bool unlimitedBlocks = false;
    public int blocksLeft = 0;
 
    //If the player wants to lock the cursor somewhere else instead, they can do that.
    public bool lockCursor = true;
 
    //static reference to the script. This is so blocks can be added via a static method.
    static BlockPlacer obj;
 
    //The block prefab to instantiate
    public GameObject blockPrefab;
	
	//The transparent prefab to instantiate:
	public GameObject highlightBlockPrefab;
 
    //Maximum range that the player can place a block from
    public float range = 7f;
	
	public GameObject tutorialBlock;
	public string Difficulty;
	
	//define 3d array for model
	int[,,] model_easy = new int[,,] {{{1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }, { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1}}};
	int[,,] model_medium = new int[,,] {{{0, 1, 1, 1}, {1, 1, 0, 0}, {0, 0, 1, 1}, {1, 1, 1, 0}}, {{0, 1, 1, 0}, {1, 1, 0, 0}, {0, 0, 1, 0}, {0, 1, 1, 0}}, {{0, 0, 1, 0}, {1, 1, 0, 0}, {0, 0, 1, 0}, {0, 1, 0, 0}}};
	int[,,] model_hard = new int[,,] {{{1, 1, 0, 1, 1}, {0, 0, 1, 0, 0}, {0, 0, 1, 0, 0}, {1, 1, 0, 1, 1}}, {{0, 1, 0, 1, 1}, {0, 0, 1, 0, 0}, {0, 0, 1, 0, 0}, {1, 1, 0, 1, 0}}, {{0, 1, 0, 1, 0}, {0, 0, 1, 0, 0}, {0, 0, 1, 0, 0}, {0, 1, 0, 1, 0}}, {{0, 1, 0, 1, 0}, {0, 0, 0, 0, 0}, {0, 0, 0, 0, 0}, {0, 1, 0, 1, 0}}};
	
	int[,,] model = new int[,,]{};
	
	//counter to keep track of modelVectors
	int counter = 0;
	
	//create list to hold model vectors:
	List<Vector3> modelVectors = new List<Vector3>();
	
	void placeBlock(Vector3 vector) {
		
		//Round the placement so the blocks snap to the grid
	    Vector3 placeAt = new Vector3(Mathf.Round(vector.x / Grid.x) * Grid.x,
	                       Mathf.Round(vector.y / Grid.y) * Grid.y,
	                       Mathf.Round(vector.z / Grid.z) * Grid.z);

        //Instantiate the block and save it so that we can do other stuff with it later
        GameObject block = (GameObject)GameObject.Instantiate(tutorialBlock, placeAt, Quaternion.Euler(Vector3.zero));
	
	}
	
	
	bool isValidVector(Vector3 vector) {
		
		if (vector.x >= (GameObject.Find("PlayArea").transform.position.x - GameObject.Find("PlayArea").transform.localScale.x) && 
			vector.x <= (GameObject.Find("PlayArea").transform.position.x + GameObject.Find("PlayArea").transform.localScale.x) &&
			vector.y >= (.995) && vector.y <= (2) &&
			vector.z >= (GameObject.Find("PlayArea").transform.position.z - GameObject.Find("PlayArea").transform.localScale.z) &&
			vector.z <= (GameObject.Find("PlayArea").transform.position.z + GameObject.Find("PlayArea").transform.localScale.z)
			) 
		{
			return true;
		}
		
		return false;
	}
 
    void Start () {

    	//This assigns the static reference
    	BlockPlacer.obj = this;
		
		
		//get difficulty setting from script variables
		if (Difficulty == "easy"){
			model = model_easy;
		}
		if (Difficulty == "medium"){
			model = model_medium;
		}
		if (Difficulty == "hard"){
			model = model_hard;
		}
		else {
			Debug.Log("Invalid difficulty setting: options are easy, medium, hard");
		}	
		
		//traverse through 3D model array and put the vectors into a list
		
		//Define the reference position:
		float origin_x = 0f;
		float origin_y = -.6f;
		float origin_z = 1.05f;
		
		int uBound0 = model.GetUpperBound(0);
		int uBound1 = model.GetUpperBound(1);
		int uBound2 = model.GetUpperBound(2);
		
		//Iterate through the 3D model array and place blocks where a 1 appears
		for (int z = 0; z <= uBound0; z++) {
			for (int y = 0; y <= uBound1; y++) {
				for (int x = 0; x <= uBound2; x++) {

					if (model[z, y, x] == 1) {
						
						Vector3 newVector = new Vector3(origin_x + (float)(x * .15),origin_z + (float)(z * .15),origin_y + (float)(y * .15));
						
						//add the vector to the list:
						modelVectors.Add(newVector);
						
					}
				}
			}
		}
		
		//add the first block of the model:
		
		//Round the placement so the blocks snap to the grid
	    Vector3 placeModel = new Vector3(Mathf.Round(modelVectors[counter].x / Grid.x) * Grid.x,
	                       Mathf.Round(modelVectors[counter].y / Grid.y) * Grid.y,
	                       Mathf.Round(modelVectors[counter].z / Grid.z) * Grid.z);

        //Instantiate the block and save it so that we can do other stuff with it later
        modelBlock = (GameObject)GameObject.Instantiate(tutorialBlock, placeModel, Quaternion.Euler(Vector3.zero));
		
		counter += 1;
		
    }
	
	
	//When user does a thing:
	//Iterate through array until 1 is located, instantiate this block and stop

    void Update () {
	
	
		//Block highlight system:
		if (blockHighlight) {
		
        	//Make a ray and raycasthit for a raycast
        	Ray ray1 = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        	RaycastHit hit1;

        	//Perform the raycast
        	if (Physics.Raycast(ray1, out hit1, range)) {

				Vector3 placeAt = ray1.GetPoint(hit1.distance - 0.1f);
		
				currentPotentialBlock = new Vector3(Mathf.Round(placeAt.x / Grid.x) * Grid.x,
			                       Mathf.Round(placeAt.y / Grid.y) * Grid.y,
			                       Mathf.Round(placeAt.z / Grid.z) * Grid.z);
				
				
		
				//make sure the vector is within the play bounds
				if (isValidVector(placeAt)) {
		
					//check if the potential block position has changed:
					if (currentPotentialBlock != prevPotentialBlock) {
			
						//destory the old highlightBlock
						Destroy(highlightBlock);
			
						//Instantiate a new highlightBlock using the currentPotentialBlock vector
						highlightBlock = (GameObject)GameObject.Instantiate(highlightBlockPrefab, currentPotentialBlock, Quaternion.Euler(Vector3.zero));
			
						//set the previous potentialBlock to the current one
						prevPotentialBlock = currentPotentialBlock;
			
					}
				}
				else {
					Destroy(highlightBlock);
				}
			}
		}

	    //Locks the cursor. Running this in update is only done because of weird unity editor shenanigans with cursor locking
   
	    if (lockCursor)
            /// ERRORS? Are you getting errors on the following line of code?
            /// If so, you're probably using Unity 4 or earlier.
            /// To fix this, add // to the start of the following line
	        Cursor.lockState = CursorLockMode.Locked;
            /// And remove // from the start of the next line of code
            //Cursor.lockCursor = true;

        //Make sure the player has enough blocks
        if (blocksLeft > 0 || unlimitedBlocks) {
            //Place blocks using the LMB
            if (Input.GetMouseButtonDown(0)) {
                //Make a ray and raycasthit for a raycast
                Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
                RaycastHit hit;

                //Perform the raycast
                if (Physics.Raycast(ray, out hit, range)) {
			
                    //The raycast is backed up so that placing works and won't place blocks inside of the ground.
                    //After testing, 0.2 units back had the best result
                    Vector3 backup = ray.GetPoint(hit.distance - 0.1f);
			
					//Round the placement so the blocks snap to the grid
				    Vector3 placeAt = new Vector3(Mathf.Round(backup.x / Grid.x) * Grid.x,
				                       Mathf.Round(backup.y / Grid.y) * Grid.y,
				                       Mathf.Round(backup.z / Grid.z) * Grid.z);
				
					//make sure the vector is within the play bounds
					if (isValidVector(placeAt)) {
						
						
						
						//iterate through the tutorial:
						if (counter < modelVectors.Count) {
							
							Debug.Log("counter is: " + counter + " and list size is: " + modelVectors.Count);
							//placeBlock(modelVectors[counter]);
							
							Destroy(modelBlock);
							
							//Round the placement so the blocks snap to the grid
						    Vector3 placeModel = new Vector3(Mathf.Round(modelVectors[counter].x / Grid.x) * Grid.x,
						                       Mathf.Round(modelVectors[counter].y / Grid.y) * Grid.y,
						                       Mathf.Round(modelVectors[counter].z / Grid.z) * Grid.z);

					        //Instantiate the block and save it so that we can do other stuff with it later
					        modelBlock = (GameObject)GameObject.Instantiate(tutorialBlock, placeModel, Quaternion.Euler(Vector3.zero));
							
							counter += 1;
							
						}
					
					
	                    //Instantiate the block and save it so that we can do other stuff with it later
	                    GameObject block = (GameObject)GameObject.Instantiate(blockPrefab, placeAt, Quaternion.Euler(Vector3.zero));

						//Debug.Log("Vector x: " + placeAt.x);
						//Debug.Log("Vector y: " + placeAt.y);
						//Debug.Log("Vector z: " + placeAt.z);
						

	                    //Remove a block from the player's "inventory"
	                    if (!unlimitedBlocks)
	                        blocksLeft--;

	                    //If the block collides with the player, remove it. We don't want the player to get stuck in their own blocks.
	                    if (block.GetComponent<Collider>().bounds.Intersects(this.transform.parent.GetComponent<Collider>().bounds)) {
	                        //If they are, destroy the block
	                        GameObject.Destroy(block);
	                        //Make sure that the player gets their misplaced block back.
	                        if (!unlimitedBlocks)
	                            blocksLeft++;
	                    }
					
					}
				
					else {
						Debug.Log("Error: Out of Bounds.");
					}

                }
            }
        }

        //Destroy blocks using the LMB
        if (Input.GetMouseButtonDown(1)) {
	
            //Make a ray and raycasthit for a raycast
            Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
            RaycastHit hit;

            //Perform the raycast
            if (Physics.Raycast(ray, out hit, range)) {	
				//Only cubes can be destroyed:
				if (hit.collider.gameObject.name == "Cube(Clone)") {
					Debug.Log(hit.collider.gameObject);
					Destroy(hit.collider.gameObject);
				}
			}
		}

    }
 
    void OnGUI()
    {
        //This is the crosshair
        GUI.Box(new Rect(Screen.width / 2 - 5, Screen.height / 2 - 5, 5, 5), "");
    }
 
    //Static adding of blocks. This isn't needed but definately helps a lot
    public static void addBlocks(int i = 1){
        BlockPlacer.obj.blocksLeft += i;
    }
}