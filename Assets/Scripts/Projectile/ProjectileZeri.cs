using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileZeri : Projectile
{
    protected override void Duration()
    {
        base.Duration();
    }


    protected override void Move()
    {
        RaycastHit hit = new RaycastHit();

        transform.position += transform.forward * m_speed * Time.deltaTime;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, m_layer))
        {

            if (Vector3.Distance(transform.position, hit.point) < 0.9f)
            {
                transform.position += (transform.position - hit.point).normalized * (1.0f - Vector3.Distance(transform.position, hit.point));
                return;
            }

            if (Vector3.Distance(transform.position, hit.point) > 1.1f)
            {
                transform.position += (hit.point - transform.position).normalized * (Vector3.Distance(transform.position, hit.point) - 1.0f);
            }
        }
    }

}
