using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Physics2DRaycasterTests
{
    GameObject m_CamGO;
    SpriteRenderer m_RedSprite;
    SpriteRenderer m_BlueSprite;
    SpriteRenderer m_GreenSprite;
    EventSystem m_EventSystem;

    [SetUp]
    public void TestSetup()
    {
        m_CamGO = new GameObject("Physics2DRaycaster Camera");
        m_CamGO.transform.position = new Vector3(0, 0, -10);
        m_CamGO.transform.LookAt(Vector3.zero);
        var cam = m_CamGO.AddComponent<Camera>();
        cam.orthographic = true;
        m_CamGO.AddComponent<Physics2DRaycaster>();
        m_EventSystem = m_CamGO.AddComponent<EventSystem>();

        var texture = new Texture2D(64, 64);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

        m_RedSprite = CreateTestSprite("Red", Color.red, sprite);
        m_BlueSprite = CreateTestSprite("Blue", Color.blue, sprite);
        m_GreenSprite = CreateTestSprite("Green", Color.green, sprite);
    }

    static SpriteRenderer CreateTestSprite(string name, Color color, Sprite sprite)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        go.AddComponent<BoxCollider2D>();
        return sr;
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(m_CamGO);
        GameObject.DestroyImmediate(m_RedSprite.gameObject);
        GameObject.DestroyImmediate(m_BlueSprite.gameObject);
        GameObject.DestroyImmediate(m_GreenSprite.gameObject);
    }

    static void AssertRaycastResultsOrder(List<RaycastResult> results, params SpriteRenderer[] expectedOrder)
    {
        Assert.AreEqual(expectedOrder.Length, results.Count);

        for (int i = 0; i < expectedOrder.Length; ++i)
        {
            Assert.AreSame(expectedOrder[i].gameObject, results[i].gameObject, "Expected {0} at index {1} but got {2}", expectedOrder[i], i, results[i].gameObject);
        }
    }

    List<RaycastResult> PerformRaycast()
    {
        var results = new List<RaycastResult>();
        var pointerEvent = new PointerEventData(m_EventSystem)
        {
            position = new Vector2(Screen.width / 2f, Screen.height / 2f)
        };

        m_EventSystem.RaycastAll(pointerEvent, results);
        return results;
    }

    [Test]
    public void RaycastAllResultsAreSortedByRendererSortingOrder()
    {
        m_RedSprite.sortingOrder = -10;
        m_BlueSprite.sortingOrder = 0;
        m_GreenSprite.sortingOrder = 5;

        var results = PerformRaycast();
        AssertRaycastResultsOrder(results, m_GreenSprite, m_BlueSprite, m_RedSprite);
    }

    [Test]
    public void RaycastAllResultsAreSortedBySortGroupOrder()
    {
        var blueSg = m_BlueSprite.gameObject.AddComponent<SortingGroup>();
        blueSg.sortingLayerID = 0;
        blueSg.sortingOrder = -10;

        var redSg = m_RedSprite.gameObject.AddComponent<SortingGroup>();
        redSg.sortingLayerID = 0;
        redSg.sortingOrder = 10;

        SortingGroup.UpdateAllSortingGroups();

        var results = PerformRaycast();
        AssertRaycastResultsOrder(results, m_RedSprite, m_GreenSprite, m_BlueSprite);
    }

    [Test]
    public void RaycastAllResultsAreSortedBySortGroupOrderAndSortingOrder()
    {
        m_RedSprite.sortingOrder = -10;
        m_BlueSprite.sortingOrder = 0;
        m_GreenSprite.sortingOrder = 5;

        var sg = m_BlueSprite.gameObject.AddComponent<SortingGroup>();
        sg.sortingLayerID = 0;
        sg.sortingOrder = 100;
        SortingGroup.UpdateAllSortingGroups();

        var results = PerformRaycast();
        AssertRaycastResultsOrder(results, m_BlueSprite, m_GreenSprite, m_RedSprite);
    }
}
