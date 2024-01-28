using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimation : MonoBehaviour
{
    private Animator _mAnimator;
    private NPCController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _mAnimator = GetComponent<Animator>();
        _controller = GetComponent<NPCController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.Agent.velocity.magnitude >= Mathf.Epsilon) {
                Debug.Log("Maior");
                _mAnimator.SetBool("Idle", false);
                _mAnimator.SetBool("Walk", true);
        } else {
            Debug.Log("Menor");
            _mAnimator.SetBool("Walk", false);
            _mAnimator.SetBool("Idle", true);
        }
    }
}
