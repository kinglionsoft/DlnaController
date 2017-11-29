const path = require('path');
const defaults = require('@ionic/app-scripts/config/webpack.config');

module.exports = function () {
  [defaults.prod, defaults.dev]
    .forEach(config => {
      if (!config.resolve.alias) {
        config.resolve.alias = {};
      }
      config.resolve.alias.kl = path.resolve('src/kl');
    })

  return defaults;
};
