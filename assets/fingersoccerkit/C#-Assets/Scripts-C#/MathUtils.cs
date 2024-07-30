using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtils
{
    public static float ClampAngle(float angle, float minAngle, float maxAngle)
    {
        angle %= (Mathf.PI * 2f);
        minAngle %= (Mathf.PI * 2f);
        maxAngle %= (Mathf.PI * 2f);

        if (IsAngleBetween(angle, minAngle, maxAngle))
        {
            return angle;
        }

        return AngleDelta(angle, minAngle) < AngleDelta(angle, maxAngle) ? minAngle : maxAngle;
    }
    
    public static bool IsAngleBetween(float target, float minAngle, float maxAngle) 
    {
        var twoPi = Mathf.PI * 2f;
        var rAngle = ((maxAngle - minAngle) % twoPi + twoPi) % twoPi;
        if (rAngle >= Mathf.PI)
        {
            (minAngle, maxAngle) = (maxAngle, minAngle);
        }

        if (minAngle <= maxAngle)
        {
            return target >= minAngle && target <= maxAngle;
        }
        
        return target >= minAngle || target <= maxAngle;
    } 

    public static float AngleDelta(float angleA, float angleB)
    {
        return  Mathf.PI - Mathf.Abs(Mathf.Abs(angleA - angleB) -  Mathf.PI);
    }
    
    public static float GetAngle(Vector3 vector)
    {
        vector.z = 0;
        var direction = vector.normalized;
        var acos = Mathf.Acos(direction.x);
        if (vector.y < 0)
        {
            acos *= -1f;
        }
        return acos;
    }

    public static (float, float) GetAllowedAnglesRange(Vector3 point, float radius, Rect rect)
    {
        var fullRange =  (0f, Mathf.PI * 2f);
        var intersentionAngles = GetIntersecionAndles(point, radius, rect);
        if (intersentionAngles.Count == 0)
        {
            return fullRange;
        }
        
        for (int i = 0; i < intersentionAngles.Count; i++)
        {
            var angle = intersentionAngles[i] % (Mathf.PI * 2);
            if (angle < 0f)
            {
                angle = Mathf.PI * 2f + angle;
            }

            intersentionAngles[i] = angle;
        }
        intersentionAngles.Sort();
        
        var pairs = new List<(float, float)>();
        for (int i = 0; i < intersentionAngles.Count; i++)
        {
            if (i < intersentionAngles.Count - 1)
            {
                pairs.Add((intersentionAngles[i], intersentionAngles[i + 1]));
            }
            else
            {
                pairs.Add((intersentionAngles[i] - Mathf.PI * 2f, intersentionAngles[0]));
            }
        }

        if (intersentionAngles.Count > 2)
        {
            var pairsInside = new List<(float, float)>();
            foreach (var (minAngle, maxAngle) in pairs)
            {
                var midAngle = Mathf.Lerp(minAngle, maxAngle, 0.5f);
                var midPoint = GetPositionOnCircle(point, radius, midAngle);
                if (rect.Contains(midPoint))
                {
                    pairsInside.Add((midAngle, maxAngle));
                }
            }

            if (pairs.Count > 0)
            {
                return pairsInside.OrderBy(pair => AngleDelta(pair.Item1, pair.Item2)).Last(); 
            }
        }

        foreach (var (minAngle, maxAngle) in pairs)
        {
            var midAngle = Mathf.Lerp(minAngle, maxAngle, 0.5f);
            var midPoint = GetPositionOnCircle(point, radius, midAngle);
            if (rect.Contains(midPoint))
            {
                return (minAngle, maxAngle);
            }
        }

        return fullRange;
    }

    public static List<float> GetIntersecionAndles(Vector3 point, float radius, Rect rect)
    {
        var intersectionAngles = new List<float>();
        if (point.x - rect.min.x < radius) // Intersect with left
        {
            var cos = (rect.min.x - point.x) / radius;
            var angle = Mathf.Acos(cos);
            
            intersectionAngles.Add(-angle);
            intersectionAngles.Add(angle);
        }
        else if (rect.max.x - point.x < radius) // Intersect with right
        {
            var cos = (rect.max.x - point.x) / radius;
            var angle = Mathf.Acos(cos);
            
            intersectionAngles.Add(angle);
            intersectionAngles.Add(Mathf.PI * 2f - angle);
        }
        
        if (point.y - rect.min.y < radius) // Intersect with bottom
        {
            var sin = (rect.min.y - point.y) / radius;
            var angle = Mathf.Asin(sin);
            
            intersectionAngles.Add(Mathf.PI - angle);
            intersectionAngles.Add(Mathf.PI * 2f + angle);
        }
        else if (rect.max.y - point.y < radius) // Intersect with top
        {
            var sin = (rect.max.y - point.y) / radius;
            var angle = Mathf.Asin(sin);
            
            intersectionAngles.Add(angle);
            intersectionAngles.Add(Mathf.PI - angle);
        }

        return intersectionAngles;
    }

    public static Vector3 GetPositionOnCircle(Vector3 center, float radius, float andleRad)
    {
        return center + new Vector3(Mathf.Cos(andleRad), Mathf.Sin(andleRad), 0f) * radius;
    }
}