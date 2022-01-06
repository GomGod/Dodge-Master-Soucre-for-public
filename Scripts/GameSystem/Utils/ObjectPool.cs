
 using System.Collections.Generic;
 using System.Linq;
 using UnityEngine;
 using Unity;

 public class ObjectPool
 {
     private GameObject _prefab;
     private readonly List<GameObject> _activePool = new();
     private readonly List<GameObject> _sleepPool = new();

     public void InitObjectPool(GameObject pfb, int genCount = 0)
     {
         _prefab = pfb;
         if (genCount > 0)
             GenerateObject(genCount);
         else
         {
             GenerateObject();
         }
     }

     public GameObject ActivateObject()
     {
         if (_prefab is null)
         {
             return null;
         }
         
         if (_sleepPool.Count <= 0)
         {
             GenerateObject();
         }

         var activeTarget = _sleepPool[0]; 
         _activePool.Add(activeTarget);
         _sleepPool.RemoveAt(0);

         activeTarget.SetActive(true);
         return activeTarget;
     }

     private void CheckSleepingInActivePool()
     {
         if (_prefab is null)
         {
             return;
         }
         
         var sleepingList = _activePool.Where(a => !a.activeSelf).ToList();
         _activePool.RemoveAll(sleepingList.Contains);
         _sleepPool.AddRange(sleepingList);
     }

     private void GenerateObject(int count=10)
     {
         if (_prefab is null)
         {
             return;
         }
         
         CheckSleepingInActivePool();
         if (_sleepPool.Count > 0)
             return;

         for (var i = 0; i < count; i += 1)
         {
             var iObj = Object.Instantiate(_prefab, GameManager.Instance.objectsContainer);
             iObj.SetActive(false);
             _sleepPool.Add(iObj);
         }
     }
 }