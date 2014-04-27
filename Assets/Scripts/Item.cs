using UnityEngine;
using System.Collections;

public abstract class Item : CustomBehaviour {
	
	internal Transform fx;	
	internal Transform xform;
	internal Vector2 dir;
	
	//--------------------------------------------------------------------------------
	// GENERIC ITEM INTERFACE
	//--------------------------------------------------------------------------------
	
	public virtual void Init() {}
	public abstract void Operate(Vector2 dir);
	public virtual void OnDrawGizmosEquipped() {}
	
	internal void SetDir(Vector2 d)
	{
		dir = d;
		fx.localRotation = Quaternion.FromToRotation (new Vector3 (1, 0, 0), new Vector3 (dir.x, dir.y, 0));
	}
	
	//--------------------------------------------------------------------------------
	// POOLING
	//--------------------------------------------------------------------------------
	
	Item prefab;
	Item next;


	public bool IsPrefab { 
		get { return prefab == null; } 
	}
	
	
	public Item Alloc(Vector2 pos) {
		Assert(IsPrefab);
		
		Item result;
		
		if (next != null) {
			
			// RECYCLE INSTANCE
			result = next;
			next = result.next;
			result.next = null;
			result.xform.position = pos;
			result.gameObject.SetActive(true);
			
		} else {
			
			// CREATE NEW INSTANCE
			result = Dup(this, pos);
			result.xform = result.transform;
			result.fx = result.xform.GetChild(0);
			result.prefab = this;
			
		}
		
		
		// RE-INIT INSTANCE
		result.Init();
		
		return result;
	}
	
	public void Release() {
		
		if (Hero.inst && this == Hero.inst.currItem) {
			Hero.inst.currItem = null;
			fx.parent = xform;
			fx.Reset();
		}
		
		if (prefab != null) {
			
			// DEACTIVATE AND PREPEND TO PREFAB'S FREELIST
			gameObject.SetActive(false);
			next = prefab.next;
			prefab.next = this;

			
		} else if (gameObject) {
			
			// THIS OBJECT WAS NOT DYNAMICALLY CREATED
			Destroy(gameObject);
			
		}
		
	}

}
