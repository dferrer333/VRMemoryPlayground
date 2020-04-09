using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTutorial : MonoBehaviour
{
	
	public GameObject tutorialBlock;
	public string Difficulty;
	
	//define 3d array for model
	int[,,] model_easy = new int[,,] { { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }, { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }};
	int[,,] model_medium = new int[,,] { { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }, { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }};
	int[,,] model_hard = new int[,,] { { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }, { {1, 1, 1, 1}, {1, 0, 0, 1}, {1, 0, 0, 1}, {1, 1, 1, 1} }};
	
	int[,,] model = new int[,,]{};
	
	//Define the grid dimensions for blocks:
	Vector3 Grid = new Vector3(0.15f, 0.15f, 0.15f);
	
	void placeBlock(Vector3 vector) {
		
		//Round the placement so the blocks snap to the grid
	    Vector3 placeAt = new Vector3(Mathf.Round(vector.x / Grid.x) * Grid.x,
	                       Mathf.Round(vector.y / Grid.y) * Grid.y,
	                       Mathf.Round(vector.z / Grid.z) * Grid.z);

        //Instantiate the block and save it so that we can do other stuff with it later
        GameObject block = (GameObject)GameObject.Instantiate(tutorialBlock, placeAt, Quaternion.Euler(Vector3.zero));
	
	}
	
    // Start is called before the first frame update
    void Start()
    {
		
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
						placeBlock(newVector);
						
					}
				}
			}
		}
		
    }

    // Update is called once per frame
    void Update()
    {
		
		
    }
}
