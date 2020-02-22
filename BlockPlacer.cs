using UnityEngine;
using System.Collections;

public class BlockPlacer : MonoBehaviour {

	public Material highlightCubeMaterial;
	public Material cubeMaterial;

    //Handles # of blocks the user has
    public bool unlimitedBlocks = false;
    public int blocksLeft = 0;

    //If the player wants to lock the cursor somewhere else instead, they can do that.
    public bool lockCursor = true;

    //static reference to the script. This is so blocks can be added via a static method.
    static BlockPlacer obj;

    //The block prefab to instantiate
    public GameObject blockPrefab;

    //Maximum range that the player can place a block from
    public float range = 7f;

        void Start () {
        //This assigns the static reference
        BlockPlacer.obj = this;
        }

        void Update () {





        	//Make a ray and raycasthit for a raycast
      //   	Ray ray1 = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
      //   	RaycastHit hit1;
			// GameObject previousObject = null;
      //
      //   	//Perform the raycast
      //   	if (Physics.Raycast(ray1, out hit1, range))
      //   	{
			// 	//don't highlight the ground
			// 	if (hit1.collider.gameObject.name != "Ground")
			// 	{
      //
			// 		GameObject currentObject = hit1.collider.gameObject;
      //
			// 		//view has switched to a different cube
			// 		if (currentObject != previousObject)
			// 		{
			// 			Debug.Log("looking at new cube");
			// 			//return color of previousObject to regular:
			// 			previousObject.GetComponent<MeshRenderer> ().material = cubeMaterial;
			// 			//set color of currentObject to highlight:
			// 			currentObject.GetComponent<MeshRenderer> ().material = highlightCubeMaterial;
      //
			// 			//set currentObject as previous:
			// 			previousObject = currentObject;
			// 		}
      //
			// 	}
			// }	






        //Locks the cursor. Running this in update is only done because of weird unity editor shenanigans with cursor locking

        if (lockCursor)
                        /// ERRORS? Are you getting errors on the following line of code?
                        /// If so, you're probably using Unity 4 or earlier.
                        /// To fix this, add // to the start of the following line
            Cursor.lockState = CursorLockMode.Locked;
                        /// And remove // from the start of the next line of code
                        //Cursor.lockCursor = true;

        //Make sure the player has enough blocks
        if (blocksLeft > 0 || unlimitedBlocks)
        {
            //Place blocks using the LMB
            if (Input.GetMouseButtonDown(0))
            {
                //Make a ray and raycasthit for a raycast
                Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
                RaycastHit hit;

                //Perform the raycast
                if (Physics.Raycast(ray, out hit, range))
                {

                    //The raycast is backed up so that placing works and won't place blocks inside of the ground.
                    //After testing, 0.2 units back had the best result
                    Vector3 backup = ray.GetPoint(hit.distance - 0.2f);

					Vector3 Grid = new Vector3(0.5f, 0.5f, 0.5f);

				    Vector3 placeAt = new Vector3(Mathf.Round(backup.x / Grid.x) * Grid.x,
				                       Mathf.Round(backup.y / Grid.y) * Grid.y,
				                       Mathf.Round(backup.z / Grid.z) * Grid.z);

                    //Round the placement so they place like blocks should
                    //Vector3 placeAt = new Vector3(Mathf.RoundToInt((backup.x / Grid.x) * Grid.x), Mathf.RoundToInt((backup.y / Grid.y) * Grid.y), Mathf.RoundToInt((backup.z / Grid.z) * Grid.z));

                    //Instantiate the block and save it so that we can do other stuff with it later
                    GameObject block = (GameObject)GameObject.Instantiate(blockPrefab, placeAt, Quaternion.Euler(Vector3.zero));

                    //Remove a block from the player's "inventory"
                    if (!unlimitedBlocks)
                        blocksLeft--;

                    //If the block collides with the player, remove it. We don't want the player to get stuck in their own blocks.
                    if (block.GetComponent<Collider>().bounds.Intersects(this.transform.parent.GetComponent<Collider>().bounds))
                    {
                        //If they are, destroy the block
                        GameObject.Destroy(block);
                        //Make sure that the player gets their misplaced block back.
                        if (!unlimitedBlocks)
                            blocksLeft++;
                    }
                }
            }
        }

        //Destroy blocks using the LMB
        if (Input.GetMouseButtonDown(1))
        {

            //Make a ray and raycasthit for a raycast
            Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
            RaycastHit hit;

            //Perform the raycast
            if (Physics.Raycast(ray, out hit, range))
            {
				//make sure the player can't destroy the ground
				if (hit.collider.gameObject.name != "Ground")
				{
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
