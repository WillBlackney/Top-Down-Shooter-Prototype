using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators;
using UnityEngine.UI;
using TMPro;

namespace GameEngine
{
    public class ArenaWaveSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private List<WaveData> waves;
        [SerializeField] private TextMeshProUGUI waveCountText;
        [SerializeField] private WaveData loopedWave;

        private int currentWaveNumber = 0;
        private float countdownToNextWave = 0;
        private void Update()
        {
            countdownToNextWave -= Time.deltaTime;
            if(countdownToNextWave < 0)
            {
                currentWaveNumber += 1;
                WaveData nextWave = loopedWave;
                if (waves.Count > 0 )
                {
                    nextWave = waves[0];
                }
                
                countdownToNextWave = nextWave.waveDuration;
                StartCoroutine(SpawnWave(nextWave));
            }

            if(waveCountText != null )
            {
                string waveNumber = currentWaveNumber.ToString();
                waveCountText.text = "Wave " + waveNumber + ", next wave in " + ((int)countdownToNextWave).ToString();
            }
           
        }
        public IEnumerator SpawnWave(WaveData wave)
        {
            if(wave == null )
            {
                yield break;
            }

            if(waves.Contains(wave))
            {
                waves.Remove(wave);
            }

            for (int i = 0; i < wave.enemiesInWave.Length; i++)
            {
                yield return new WaitForSeconds(wave.enemiesInWave[i].spawnDelay);

                Vector3 spawnPoint = GetRandomValidSpawnPoint();
                Instantiate(wave.enemiesInWave[i].prefabSpawned, spawnPoint, Quaternion.identity);
            }

            yield return null;
        }

        private Vector3 GetRandomValidSpawnPoint()
        {
            Character player = Array.Find(FindObjectsOfType<Character>(), character => character.CharacterType == Character.CharacterTypes.Player);
            float minDistance = 3f;
            var possibleSpawnLocations = Array.FindAll(spawnPositions, node =>
                Vector2.Distance(node.transform.position, player.transform.position) >= minDistance);

            return possibleSpawnLocations.GetRandomElement().position;
        }
    }
    [System.Serializable]
    public class WaveData
    {
        public float waveDuration = 10f;
        public EnemySpawnData[] enemiesInWave;
    }
    [System.Serializable]
    public class EnemySpawnData
    {
        public float spawnDelay = 2f;
        public GameObject prefabSpawned;
    }
}