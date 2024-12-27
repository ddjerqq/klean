const TerserPlugin = require("terser-webpack-plugin");

/** @type {import('webpack').Configuration} */
module.exports = {
  mode: "production",
  entry: "./wwwroot/scripts/globe.ts",
  devtool: "source-map",
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: ['.ts', '.js'],
  },
  output: {
    path: __dirname + "/wwwroot/scripts",
    filename: "globe.min.js",
  },
  optimization: {
    minimize: true,
    minimizer: [
      new TerserPlugin({
        terserOptions: {
          keep_classnames: true,
          keep_fnames: true
        }
      })
    ]
  }
}