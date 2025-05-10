namespace TelemetryManagerExamples
{
    using USCG.Core.Telemetry;

    using UnityEngine;

    public class RecordMetricsExample : MonoBehaviour
    {
        // Keep a reference to the metrics we create.
        private MetricId _deltaTimeMetric = default;
        private MetricId _shiftMetric = default;

        private void Start()
        {
            // Create all metrics in Start().
            _deltaTimeMetric = TelemetryManager.instance.CreateSampledMetric<float>("deltaTimeMetric");
            _shiftMetric = TelemetryManager.instance.CreateAccumulatedMetric("ShiftMetric");
        }

        private void Update()
        {
            // There are a number of places where you can call these functions. This example only illustrates how you record metrics,
            // but it's up to you to decide when you want to record your metrics!

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                // This will increment the provided metric by 1.
                TelemetryManager.instance.AccumulateMetric(_shiftMetric, 1);
            }

            TelemetryManager.instance.AccumulateMetric(_deltaTimeMetric, Time.deltaTime);

            // If you uncomment this line you'll see a sample get created for every frame. This may be desirable in some cases, but
            // you likely want a better way to collect samples (e.g. responding to an event).
            //TelemetryManager.instance.AddMetricSample(_deltaTimeMetric, Time.deltaTime);
        }
    }
}
