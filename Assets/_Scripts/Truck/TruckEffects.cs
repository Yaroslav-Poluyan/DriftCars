using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Truck
{
    public class TruckEffects : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _tireTrailPrefab;
        [SerializeField] private ParticleSystem _tireParticlesPrefab;
        [SerializeField] private TruckController _truckController;
        [SerializeField] private PhotonView _photonView;
        private readonly List<WheelEffects> _wheelEffectsList = new();
        private const float SlipAllowance = .2f;

        #region Braking

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _brakeMaterialIdx = 4;
        [SerializeField] private Material _brakeOffMaterial;
        [SerializeField] private Material _brakeOnMaterial;

        #endregion


        private struct WheelEffects
        {
            public TrailRenderer WheelTrail;
            public ParticleSystem WheelParticles;
            public AudioSource SkidSound;
        }

        private void Start()
        {
            var wheelColliders = _truckController.Colliders;
            WheelCollider[] wheelColliderArray = new WheelCollider[]
            {
                wheelColliders._frWheel,
                wheelColliders._flWheel,
                wheelColliders._rrWheel,
                wheelColliders._rlWheel
            };

            //enable trails after delay
            StartCoroutine(EnableTrailsAferDelay(2f));

            foreach (var col in wheelColliderArray)
            {
                WheelEffects newWheelEffects;
                newWheelEffects.WheelTrail = CreateTireTrail(col);
                newWheelEffects.WheelParticles = CreateTireParticles(col);
                newWheelEffects.SkidSound = col.GetComponent<AudioSource>();
                _wheelEffectsList.Add(newWheelEffects);
            }
        }

        private IEnumerator EnableTrailsAferDelay(float f)
        {
            foreach (var effect in _wheelEffectsList)
            {
                effect.WheelTrail.emitting = false;
            }

            yield return new WaitForSeconds(f);
            foreach (var effect in _wheelEffectsList)
            {
                effect.WheelTrail.Clear();
                effect.WheelTrail.emitting = true;
            }
        }

        private TrailRenderer CreateTireTrail(WheelCollider wheelCollider)
        {
            return Instantiate(_tireTrailPrefab,
                wheelCollider.transform.position - Vector3.up * wheelCollider.radius,
                Quaternion.identity, wheelCollider.transform);
        }

        private ParticleSystem CreateTireParticles(WheelCollider wheelCollider)
        {
            return Instantiate(_tireParticlesPrefab,
                wheelCollider.transform.position - Vector3.up * wheelCollider.radius,
                Quaternion.identity, wheelCollider.transform);
        }

        public void CheckParticles()
        {
            var wheelHits = new WheelHit[4];
            var wheelColliders = _truckController.Colliders;

            wheelColliders._frWheel.GetGroundHit(out wheelHits[0]);
            wheelColliders._flWheel.GetGroundHit(out wheelHits[1]);
            wheelColliders._rrWheel.GetGroundHit(out wheelHits[2]);
            wheelColliders._rlWheel.GetGroundHit(out wheelHits[3]);

            for (int i = 0; i < wheelHits.Length; i++)
            {
                HandleWheelState(wheelHits[i], i);
            }
        }

        private void HandleWheelState(WheelHit wheelHit, int idx)
        {
            if (Mathf.Abs(wheelHit.sidewaysSlip) + Mathf.Abs(wheelHit.forwardSlip) > SlipAllowance)
            {
                _photonView.RPC(nameof(PlayEffects), RpcTarget.All, _photonView.ViewID, true, idx);
            }
            else
            {
                _photonView.RPC(nameof(PlayEffects), RpcTarget.All, _photonView.ViewID, false, idx);
            }
        }

        [PunRPC]
        private void PlayEffects(int id, bool isPlaying, int idx)
        {
            if (_photonView.ViewID != id) return;
            var wheelEffects = _wheelEffectsList[idx];
            if (isPlaying)
            {
                wheelEffects.WheelParticles.Play();
                if (!wheelEffects.SkidSound.isPlaying) wheelEffects.SkidSound.Play();
                wheelEffects.WheelTrail.emitting = true;
            }
            else
            {
                wheelEffects.WheelParticles.Stop();
                wheelEffects.SkidSound.Stop();
                wheelEffects.WheelTrail.emitting = false;
            }
        }

        public void DisableAllEffects()
        {
            foreach (var effect in _wheelEffectsList)
            {
                effect.WheelParticles.Stop();
                effect.SkidSound.Stop();
                effect.WheelTrail.emitting = false;
            }
        }

        public void OnBrake(bool state)
        {
            _meshRenderer.materials[_brakeMaterialIdx] = state ? _brakeOnMaterial : _brakeOffMaterial;
        }
    }
}