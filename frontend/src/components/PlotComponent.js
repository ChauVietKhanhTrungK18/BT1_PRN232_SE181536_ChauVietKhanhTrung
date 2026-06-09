import Plotly from 'plotly.js/dist/plotly';
import factoryModule from 'react-plotly.js/factory';

// react-plotly.js/factory uses CJS exports.default, 
// Vite may wrap it differently depending on version
const createPlotlyComponent =
  typeof factoryModule === 'function' ? factoryModule : factoryModule.default;

const Plot = createPlotlyComponent(Plotly);
export default Plot;
