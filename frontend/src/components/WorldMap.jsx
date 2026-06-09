import React, { useMemo } from 'react';
import Plot from './PlotComponent';

const METRIC_CONFIG = {
  confirmed: { field: 'confirmed', label: 'Confirmed Cases', colorscale: 'Blues' },
  active: { field: 'active', label: 'Active Cases', colorscale: 'YlOrRd' },
  recovered: { field: 'recovered', label: 'Recovered', colorscale: 'Greens' },
  deaths: { field: 'deaths', label: 'Deaths', colorscale: 'Reds' },
  dailyIncrease: { field: 'dailyIncrease', label: 'Daily Increase', colorscale: 'Purples' },
};

export default function WorldMap({ data, activeTab }) {
  const config = METRIC_CONFIG[activeTab] || METRIC_CONFIG.confirmed;

  const plotData = useMemo(() => {
    if (!data || data.length === 0) return [];

    const countryCodes = data.map((d) => d.countryCode);
    const values = data.map((d) => d[config.field] || 0);
    const labels = data.map(
      (d) =>
        `${d.country}<br>${config.label}: ${(d[config.field] || 0).toLocaleString()}`
    );

    return [
      {
        type: 'choropleth',
        locationmode: 'ISO-3',
        locations: countryCodes,
        z: values,
        text: data.map((d) => d.country),
        hovertemplate:
          '<b>%{text}</b><br>' +
          `${config.label}: %{z:,.0f}<br>` +
          '<extra></extra>',
        colorscale: config.colorscale,
        autocolorscale: false,
        reversescale: false,
        marker: {
          line: {
            color: '#c8d6e5',
            width: 0.5,
          },
        },
        colorbar: {
          title: {
            text: config.label,
            font: { size: 12, family: 'Inter' },
          },
          thickness: 15,
          len: 0.6,
          tickfont: { size: 10 },
        },
      },
    ];
  }, [data, activeTab, config]);

  const layout = useMemo(
    () => ({
      geo: {
        showframe: false,
        showcoastlines: true,
        coastlinecolor: '#c8d6e5',
        showland: true,
        landcolor: '#f0f4f8',
        showocean: true,
        oceancolor: '#e8f4fd',
        showlakes: true,
        lakecolor: '#e8f4fd',
        showcountries: true,
        countrycolor: '#c8d6e5',
        countrywidth: 0.5,
        projection: {
          type: 'natural earth',
        },
      },
      margin: { t: 10, b: 10, l: 10, r: 10 },
      height: 420,
      paper_bgcolor: 'transparent',
      plot_bgcolor: 'transparent',
      font: { family: 'Inter, sans-serif' },
    }),
    []
  );

  return (
    <div className="world-map-container">
      <Plot
        data={plotData}
        layout={layout}
        config={{
          responsive: true,
          displayModeBar: false,
          scrollZoom: false,
        }}
        style={{ width: '100%', height: '100%' }}
        useResizeHandler={true}
      />
    </div>
  );
}
