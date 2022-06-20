using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

/* fuck unity
 * fuck tilemaps
 * fuck scriptableobjects
 * fuck hashing
 * fuck rendering
 * fuck OOP
 * fuck override/virtual
 * fuck UnityEngine.Tile
 * fuck UnityEngine.TileBase
 * fuck UnityEngine
 * fuck unity editor
 * fuck unity engine
 * fuck lists
 * fuck sprites
 * fuck ref tiledata (which doesnt even exit)
 * fuck 2d-extras
 * fuck everything
 */

namespace UnityEngine.Tilemaps
{
    /// <summary>
    ///     * fuck unity
    ///     * fuck tilemaps
    ///     * fuck scriptableobjects
    ///     * fuck hashing
    ///     * fuck rendering
    ///     * fuck OOP
    ///     * fuck override/virtual
    ///     * fuck UnityEngine.Tile
    ///     * fuck UnityEngine.TileBase
    ///     * fuck UnityEngine
    ///     * fuck unity editor
    ///     * fuck unity engine
    ///     * fuck lists
    ///     * fuck sprites
    ///     * fuck ref tiledata(which doesnt even exit)
    ///     * fuck 2d-extras
    ///     * fuck everything
    /// </summary>
    /*[CreateAssetMenu]
    public class FuckBitchTile : Tile
    {
        List<Sprite> bitchSprites = new List<Sprite>();

        public void SetTheFuckUp(List<Sprite> bitchSprites)
        {
            this.bitchSprites = bitchSprites;
            base.sprite = bitchSprites[(int)(Random.value*bitchSprites.Count)];
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            Debug.Log($"IM RACITS");
            base.sprite = bitchSprites[(int)(Random.value*bitchSprites.Count)];
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = bitchSprites[(int)(Random.value*bitchSprites.Count)];
            base.GetTileData(position, tilemap, ref tileData);
            base.RefreshTile(position, tilemap);
        }*/
        /*public Sprite spr;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = spr;
            base.GetTileData(position, tilemap, ref tileData);
        }
    }*/
}


//namespace UnityEngine.Tilemaps
//{
/// <summary>
///     * fuck unity
///     * fuck tilemaps
///     * fuck scriptableobjects
///     * fuck hashing
///     * fuck rendering
///     * fuck OOP
///     * fuck override/virtual
///     * fuck UnityEngine.Tile
///     * fuck UnityEngine.TileBase
///     * fuck UnityEngine
///     * fuck unity editor
///     * fuck unity engine
///     * fuck lists
///     * fuck sprites
///     * fuck ref tiledata(which doesnt even exit)
///     * fuck 2d-extras
/// </summary>
//[Serializable]
namespace UnityEngine.Tilemaps
{
    public class FuckBitchTile : Tile
    {
        /// <summary>
        /// The Sprites used for randomizing output.
        /// </summary>
        public List<Sprite> m_Sprites = new List<Sprite>();

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            if ((m_Sprites != null) && (m_Sprites.Count > 0))
            {
                long hash = position.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= position.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                var oldState = Random.state;
                Random.InitState((int)hash);
                base.sprite = m_Sprites[Random.Range(0, m_Sprites.Count - 1)];//(int)(m_Sprites.Count * Random.value)];
                Random.state = oldState;
            }
            base.RefreshTile(position, tilemap);
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            if ((m_Sprites != null) && (m_Sprites.Count > 0))
            {
                long hash = position.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= position.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                var oldState = Random.state;
                Random.InitState((int)hash);
                tileData.sprite = m_Sprites[Random.Range(0, m_Sprites.Count - 1)];//(int)(m_Sprites.Count * Random.value)];
                Random.state = oldState;
            }
            //else
            //    Debug.LogError("penis");
        }
    }
}
/*#if UNITY_EDITOR
    [CustomEditor(typeof(RandomTile))]
    public class RandomTileEditor : Editor
    {
        private SerializedProperty m_Color;
        private SerializedProperty m_ColliderType;

        private RandomTile tile { get { return (target as RandomTile); } }

        /// <summary>
        /// OnEnable for RandomTile.
        /// </summary>
        public void OnEnable()
        {
            m_Color = serializedObject.FindProperty("m_Color");
            m_ColliderType = serializedObject.FindProperty("m_ColliderType");
        }

        /// <summary>
        /// Draws an Inspector for the RandomTile.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            int count = EditorGUILayout.DelayedIntField("Number of Sprites", tile.m_Sprites != null ? tile.m_Sprites.Count : 0);
            if (count < 0)
                count = 0;
            if (tile.m_Sprites == null || tile.m_Sprites.Count != count)
            {
                //Array.Resize<Sprite>(ref tile.m_Sprites, count);
            }

            if (count == 0)
                return;

            EditorGUILayout.LabelField("Place random sprites.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++)
            {
                tile.m_Sprites[i] = (Sprite)EditorGUILayout.ObjectField("Sprite " + (i + 1), tile.m_Sprites[i], typeof(Sprite), false, null);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_ColliderType);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(tile);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
*/
//}*/