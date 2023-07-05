using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Factory : GenericBuild, IProduce, IReceive
{
    [SerializeField] private TypeOfProduct typeOfProductReceive;
    [SerializeField] private TypeOfProduct typeOfProductProduce;

    private GameObject producePrefab;
    private GameObject receivePrefab;

    private List<Product> ironsList = new List<Product>();
    private List<Product> swordsList = new List<Product>();

    private bool isReadyToWork = false;
    private bool isCoroutineEnabled = false;

    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject receivePoint;


    private int ironOnFactory = 0;
    private int swordsOnFactory = 0;

    // Iron container config
    private float offsetXIron;
    private float offsetYIron;
    private float offsetZIron;

    private int widthLimitIron = 4;
    private int lengthLimitIron = 2;

    private float lengthIron = 0;
    private float heightIron = 0;
    private float widthIron = 0;

    // Sword container config
    private float offsetXSword;
    private float offsetYSword;
    private float offsetZSword;

    private int widthLimitSword = 3;
    private int lengthLimitSword = 3;

    private float lengthSword = 0;
    private float heightSword = 0;
    private float widthSword = 0;

    private void Start()
    {
        receivePrefab = ProductManager.Instance.ChooseProductPrefab(typeOfProductReceive);
        producePrefab = ProductManager.Instance.ChooseProductPrefab(typeOfProductProduce);

        CalculateReceiveProductSize(receivePrefab.transform);
        CalculateProduceProductSize(producePrefab.transform);
    }

    private void CalculateReceiveProductSize(Transform transform)
    {
        var boxCollider = transform.gameObject.GetComponent<BoxCollider>();

        offsetXIron = boxCollider.size.x * transform.localScale.x;
        offsetZIron = boxCollider.size.y * transform.localScale.y;
        offsetYIron = boxCollider.size.z * transform.localScale.z;
    }

    private void CalculateProduceProductSize(Transform transform)
    {
        var boxCollider = transform.gameObject.GetComponent<BoxCollider>();

        offsetXSword = boxCollider.size.x * transform.localScale.x;
        offsetZSword = boxCollider.size.y * transform.localScale.y;
        offsetYSword = boxCollider.size.z * transform.localScale.z;
    }


    private void AddReceiveProduct(Vector3 _spawnPos)
    {
        GameObject receiveObject = Instantiate(receivePrefab, _spawnPos, receivePrefab.transform.rotation);
        Product product = receiveObject.gameObject.GetComponent<Product>();
        ironsList.Add(product);
    }

    private void CalculateNewIronPosition()
    {
        lengthIron++;
        if (lengthIron > widthLimitIron)
        {
            lengthIron = 0;
            heightIron++;
            if (heightIron > lengthLimitIron)
            {
                lengthIron = 0;
                heightIron = 0;
                widthIron++;
            }
        }
    }

    private void CalculateNewSwordPosition()
    {
        lengthSword++;
        if (lengthSword > widthLimitSword)
        {
            lengthSword = 0;
            heightSword++;
            if (heightSword > lengthLimitSword)
            {
                lengthSword = 0;
                heightSword = 0;
                widthSword++;
            }
        }
    }

    public void ProduceProduct(Vector3 spawnPoint)
    {
        GameObject produceObject = Instantiate(producePrefab, spawnPoint, producePrefab.transform.rotation);
        Product product = produceObject.gameObject.GetComponent<Product>();
        swordsList.Add(product);
    }

    public void ReceiveProduct(int productAmount)
    {
        SetIronToFactory(productAmount);

        for (int i = 0; i < productAmount; i++)
        {
            PlaceNewIronAtReceivePos();
        }

        if (isReadyToWork && !isCoroutineEnabled)
        {
            StartCoroutine(FabricaConvertsIron());
        }
    }

    private void SetIronToFactory(int ironAmount)
    {
        ironOnFactory += ironAmount;
    }

    private void PlaceNewIronAtReceivePos()
    {
        CalculateNewIronPosition();
        Vector3 receivePos = receivePoint.transform.position + new Vector3(lengthIron * offsetXIron, widthIron * offsetYIron, -(heightIron * offsetZIron));
        AddReceiveProduct(receivePos);
        isReadyToWork = true;
    }

    private IEnumerator FabricaConvertsIron()
    {
        isCoroutineEnabled = true;
        while (true && isReadyToWork)
        {

            yield return new WaitForSeconds(1.0f);
            ConvertIronToSword();
        }
    }

    private void ConvertIronToSword()
    {
        Debug.Log("Convert");
        CheckRemainingIron();
        if (isReadyToWork)
        {
            RemoveIron();
            AddSword();
        }
    }

    private void RemoveIron()
    {
        Destroy(ironsList[ironsList.Count - 1].gameObject);
        ironsList.RemoveAt(ironsList.Count - 1);
    }

    private void AddSword()
    {
        CalculateNewSwordPosition();
        Vector3 swordSpawnPos = spawnPoint.transform.position + new Vector3(lengthSword * offsetXSword, widthSword * offsetYSword, -(heightSword * offsetZSword));
        ProduceProduct(swordSpawnPos);
    }

    private void CheckRemainingIron()
    {
        if (ironsList.Count <= 0)
        {
            StopCoroutine(FabricaConvertsIron());
            isReadyToWork = false;
            isCoroutineEnabled=false;
            ironsList.Clear();
        }
    }

}
