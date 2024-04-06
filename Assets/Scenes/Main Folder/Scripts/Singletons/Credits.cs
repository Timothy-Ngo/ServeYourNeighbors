// Kirin Hardinger
// March 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] GameObject logo;
    [SerializeField] GameObject institution;
    [SerializeField] GameObject titles;
    [SerializeField] GameObject names;

    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    private void FixedUpdate()
    {
        institution.transform.Translate(Vector3.up * 0.02f);
        titles.transform.Translate(Vector3.up * 0.02f);
        names.transform.Translate(Vector3.up * 0.02f);

        if(institution.transform.localPosition.y >= 50)
        {
            logo.transform.Translate(Vector3.up * 0.02f);
        }

        if (institution.transform.localPosition.y > 2600)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
