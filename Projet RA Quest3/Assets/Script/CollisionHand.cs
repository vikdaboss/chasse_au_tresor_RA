using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.UI.GridLayoutGroup;

public class CollisionHand : MonoBehaviour
{
    public Action<IInteractable, Rigidbody> WhenTriggerEntered = delegate { };
    private IInteractable _interactable;
    private Dictionary<Rigidbody, bool> _rigidbodyTriggers;
    public Animator chestAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbodyTriggers = new Dictionary<Rigidbody, bool>();
        chestAnimator.SetBool("Open", false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnTriggerStay(Collider collider)
    {
        Rigidbody rigidbody = collider.attachedRigidbody;

        if (rigidbody == null)
        {
            return;
        }

        if (!_rigidbodyTriggers.ContainsKey(rigidbody))
        {
            WhenTriggerEntered(_interactable, rigidbody);
            _rigidbodyTriggers.Add(rigidbody, true);
        }
        else
        {
            _rigidbodyTriggers[rigidbody] = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            chestAnimator.SetBool("Open", true);
        }
    }
}
