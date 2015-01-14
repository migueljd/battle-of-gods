using UnityEngine;
using UnityEditor;

using System.Collections;

using TBTK;

namespace TBTK {

	public class MenuExtension : EditorWindow {
		
		[MenuItem ("Tools/TBTK/New Scene - Square Grid", false, -100)]
		private static void NewSceneSquareGrid(){
			EditorApplication.NewScene();
			GameObject camObj=Camera.main.gameObject; 	DestroyImmediate(camObj);
			
			GameObject obj=(GameObject)Instantiate(Resources.Load("ScenePrefab/TBTK_SquareGrid", typeof(GameObject)));
			obj.name="TBTK_SquareGrid";
		}
		
		[MenuItem ("Tools/TBTK/New Scene - Hex Grid", false, -100)]
		static void NewSceneHexGrid() {
			EditorApplication.NewScene();
			GameObject camObj=Camera.main.gameObject; 	DestroyImmediate(camObj);
			
			GameObject obj=(GameObject)Instantiate(Resources.Load("ScenePrefab/TBTK_HexGrid", typeof(GameObject)));
			obj.name="TBTK_HexGrid";
		}
		
		
		
		
		
		
		
		
		
		
		
		[MenuItem ("Tools/TBTK/FactionManagerEditor", false, 10)]
		static void OpenFactionManagerEditorWindow () {
			FactionManagerEditorWindow.Init();
		}
		
		[MenuItem ("Tools/TBTK/UnitEditor", false, 10)]
		static void OpenUnitEditor () {
			UnitEditorWindow.Init();
		}
		
		[MenuItem ("Tools/TBTK/UnitAbilityEditor", false, 10)]
		public static void OpenUnitAbilityEditor () {
			UnitAbilityEditorWindow.Init();
		}
		
		[MenuItem ("Tools/TBTK/FactionAbilityEditor", false, 10)]
		public static void OpenFactionAbilityEditor () {
			FactionAbilityEditorWindow.Init();
		}
		
		[MenuItem ("Tools/TBTK/PerkEditor", false, 10)]
		public static void OpenPerkEditor () {
			PerkEditorWindow.Init();
		}
		
		[MenuItem ("Tools/TBTK/DamageArmorTable", false, 10)]
		public static void OpenDamageTable () {
			DamageArmorDBEditor.Init();
		}
		
		[MenuItem ("Tools/TBTK/Contact and Support Info", false, 100)]
		static void OpenForumLink () {
			SupportContactWindow.Init();
		}
		
	}


}