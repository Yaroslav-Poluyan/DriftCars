using System.Collections;
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

        //
        private TrailRenderer _frWheelTrail;
        private TrailRenderer _flWheelTrail;
        private TrailRenderer _rrWheelTrail;

        private TrailRenderer _rlWheelTrail;

        //
        private ParticleSystem _frWheelParticles;
        private ParticleSystem _flWheelParticles;
        private ParticleSystem _rrWheelParticles;

        private ParticleSystem _rlWheelParticles;

        //
        private AudioSource _frSkidSound;
        private AudioSource _flSkidSound;
        private AudioSource _rrSkidSound;
        private AudioSource _rlSkidSound;

        //
        private const float SlipAllowance = .2f;

        private void Start()
        {
            var wheelColliders = _truckController.Colliders;
            _frWheelTrail = CreateTireTrail(wheelColliders._frWheel);
            _flWheelTrail = CreateTireTrail(wheelColliders._flWheel);
            _rrWheelTrail = CreateTireTrail(wheelColliders._rrWheel);
            _rlWheelTrail = CreateTireTrail(wheelColliders._rlWheel);
            //enable trails after delay
            StartCoroutine(EnableTrailsAferDelay(2f));
            //
            _frWheelParticles = CreateTireParticles(wheelColliders._frWheel);
            _flWheelParticles = CreateTireParticles(wheelColliders._flWheel);
            _rrWheelParticles = CreateTireParticles(wheelColliders._rrWheel);
            _rlWheelParticles = CreateTireParticles(wheelColliders._rlWheel);
            //
            _frSkidSound = wheelColliders._frWheel.GetComponent<AudioSource>();
            _flSkidSound = wheelColliders._flWheel.GetComponent<AudioSource>();
            _rrSkidSound = wheelColliders._rrWheel.GetComponent<AudioSource>();
            _rlSkidSound = wheelColliders._rlWheel.GetComponent<AudioSource>();
        }

        private IEnumerator EnableTrailsAferDelay(float f)
        {
            _frWheelTrail.emitting = false;
            _flWheelTrail.emitting = false;
            _rrWheelTrail.emitting = false;
            _rlWheelTrail.emitting = false;
            yield return new WaitForSeconds(f);
            _frWheelTrail.Clear();
            _flWheelTrail.Clear();
            _rrWheelTrail.Clear();
            _rlWheelTrail.Clear();
            _frWheelTrail.emitting = true;
            _flWheelTrail.emitting = true;
            _rrWheelTrail.emitting = true;
            _rlWheelTrail.emitting = true;
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

            HandleWheelState(wheelHits[0], _frWheelParticles, _frWheelTrail, _frSkidSound);
            HandleWheelState(wheelHits[1], _flWheelParticles, _flWheelTrail, _flSkidSound);
            HandleWheelState(wheelHits[2], _rrWheelParticles, _rrWheelTrail, _rrSkidSound);
            HandleWheelState(wheelHits[3], _rlWheelParticles, _rlWheelTrail, _rlSkidSound);
        }

        private void HandleWheelState(WheelHit wheelHit, ParticleSystem wheelParticle, TrailRenderer wheelParticleTrail,
            AudioSource
                wheelAudioSource)
        {
            if (Mathf.Abs(wheelHit.sidewaysSlip) + Mathf.Abs(wheelHit.forwardSlip) > SlipAllowance)
            {
                wheelParticle.Play();
                if (!wheelAudioSource.isPlaying) wheelAudioSource.Play();
                wheelParticleTrail.emitting = true;
                /*
                _photonView.RPC(nameof(PlayEffects), RpcTarget.All, true,  _photonView.ViewID);
            */
            }
            else
            {
                wheelParticle.Stop();
                wheelAudioSource.Stop();
                wheelParticleTrail.emitting = false;
                /*_photonView.RPC(nameof(PlayEffects), RpcTarget.All, false, _photonView.ViewID);*/

            }
        }
        [PunRPC]
        private void PlayEffects(bool state, int id)
        {
            if (_photonView.ViewID == id)
            {
                
            }
        }

        public void DisableAllEffects()
        {
            _frWheelTrail.emitting = false;
            _flWheelTrail.emitting = false;
            _rrWheelTrail.emitting = false;
            _rlWheelTrail.emitting = false;
            //
            _frWheelParticles.Stop();
            _flWheelParticles.Stop();
            _rrWheelParticles.Stop();
            _rlWheelParticles.Stop();
            //
            _frSkidSound.Stop();
            _flSkidSound.Stop();
            _rrSkidSound.Stop();
            _rlSkidSound.Stop();
        }
    }
}