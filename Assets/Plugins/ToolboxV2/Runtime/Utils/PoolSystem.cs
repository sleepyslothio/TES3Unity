using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public class PoolSystem<T> : MonoBehaviour where T : MonoBehaviour
    {
        private Transform m_PoolTransform = null;
        private GameObject m_TempGameObject = null;
        private Transform m_TempTransform = null;
        private List<T> m_Pool = null;
        private int m_Size = 0;

        [Header("Pool Settings")]
        [SerializeField]
        protected string m_poolName = "__PoolSystem";
        [SerializeField]
        protected GameObject m_prefab;
        [SerializeField]
        protected int m_poolSize = 10;
        [SerializeField]
        protected bool m_autoResize = false;
        [SerializeField]
        private Collider[] m_excluedActors = null;

        public GameObject this[int index] { get { return m_Pool[index].gameObject; } }

        public string PoolName
        {
            get { return m_poolName; }
            set
            {
                if (m_PoolTransform == null)
                {
                    return;
                }

                m_poolName = value;
                m_PoolTransform.gameObject.name = m_poolName;
            }
        }

        public int Size { get { return m_Size; } }

        public void Resize(int size)
        {
            if (size != m_Size && m_Pool != null)
            {
                int diff = size - m_Size;

                if (diff > 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        AddPrefab();
                    }
                }
                else if (diff < 0)
                {
                    m_Pool.RemoveRange(m_Size + diff, diff);
                }

                m_Size += diff;
            }
        }

        protected virtual void Start()
        {
            m_TempGameObject = new GameObject(m_poolName);
            m_PoolTransform = m_TempGameObject.GetComponent<Transform>();
            m_Pool = new List<T>(m_poolSize);

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                AddPrefab();
            }
        }

        protected virtual GameObject AddPrefab()
        {
            m_TempGameObject = Instantiate(m_prefab, Vector3.zero, Quaternion.identity);
            m_TempGameObject.transform.parent = m_PoolTransform;
            m_TempGameObject.SetActive(false);
            m_Pool.Add(m_TempGameObject.GetComponent<T>());
            m_Size++;

            var excludeCount = m_excluedActors != null ? m_excluedActors.Length : 0;
            if (excludeCount > 0)
            {
                for (var j = 0; j < excludeCount; j++)
                {
                    Physics.IgnoreCollision(m_TempGameObject.GetComponent<Collider>(), m_excluedActors[j]);
                }
            }

            return m_TempGameObject;
        }

        public virtual GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            m_TempGameObject = GetGameObject();
            m_TempTransform = m_TempGameObject.GetComponent<Transform>();
            m_TempTransform.position = position;
            m_TempTransform.rotation = rotation;
            m_TempGameObject.SetActive(true);
            return m_TempGameObject;
        }

        protected virtual GameObject GetGameObject()
        {
            var index = GetFirstDisabled();

            if (index == -1)
            {
                if (!m_autoResize)
                {
                    Debug.LogWarning("[PoolSystem] Warning: All objects are active. The first element will be used.");
                    m_TempGameObject = m_Pool[0].gameObject;
                }
                else
                {
                    m_TempGameObject = AddPrefab();
                }
            }
            else
            {
                m_TempGameObject = m_Pool[index].gameObject;
            }

            return m_TempGameObject;
        }

        protected virtual int GetFirstDisabled()
        {
            for (int i = 0; i < m_Size; i++)
            {
                if (!m_Pool[i].gameObject.activeSelf)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}