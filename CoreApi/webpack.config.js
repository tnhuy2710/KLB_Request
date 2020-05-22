const path = require('path');
const webpack = require('webpack');

const ExtractTextPlugin = require("extract-text-webpack-plugin");
const { CheckerPlugin } = require('awesome-typescript-loader');

//Thư mục sẽ chứa tập tin được biên dịch
var distDir = path.resolve(__dirname, './wwwroot/dist');

//Thư mục chứa dự án - các component React
var scriptDir = path.resolve(__dirname, './Areas/Admin/Scripts');

const config = {
    entry: {
        homeIndex: path.join(scriptDir, "Components/Home/Index.tsx"),
        
        site: [
            "./adminlte-ui/dist/css/AdminLTE.min.css",
            "./adminlte-ui/dist/css/skins/skin-blue.min.css",
            "./adminlte-ui/dist/js/adminlte.min.js",
            "./wwwroot/css/admin/site.css"
            ],
        libs: [
            'bootstrap',
            'bootstrap/dist/css/bootstrap.css',
            'font-awesome/css/font-awesome.css',
            './semantic-ui/dist/semantic.min.css',
        ],
        vendors: [
            'react',
            'react-dom',
            'jquery',
            'semantic-ui-react'
        ]
    },
    output: {
        path: distDir,
        filename: '[name].js',
        publicPath: '/dist/'
    },
    module: {
        rules: [
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|eot|ttf)$/,
                use: [
                    {
                        loader: 'url-loader',
                        options: {
                            limit: 50000,
                            name: 'assets/[name]_[hash].[ext]'
                        }
                    }
                ]
            },
            {
                test: /\.ts(x?)$/,
                exclude: /node_modules/,
                use: [
                    { loader: 'babel-loader' },
                    {
                        loader: 'awesome-typescript-loader',
                        options: {
                            silent: true
                        }
                    },
                ]
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: [
                    { loader: 'babel-loader' }
                ]
            },
            {
                test: /\.(css|scss)$/,
                use: ExtractTextPlugin.extract({
                    fallback: 'style-loader',
                    use: ['css-loader', 'sass-loader']
                })
            },
            {
                test: /[\/\\]node_modules[\/\\]some-module[\/\\]index\.js$/,
                loader: "imports-loader?this=>window"
            }
        ]
    },
    plugins: [
        new CheckerPlugin(),
        new ExtractTextPlugin({
            filename: 'styles/[name].css',
            allChunks: true
        }),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery'
        }),
        new webpack.optimize.CommonsChunkPlugin({
            name: "vendors",
            filename: "vendors.bundle.js"
        }),
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('production')
            }
        }),
        new webpack.optimize.UglifyJsPlugin(),
        //new webpack.SourceMapDevToolPlugin({
        //    moduleFilenameTemplate: path.relative(distDir, '[resourcePath]')
        //}),
        //new CopyWebpackPlugin([
        //    { from: './node_modules/', to: 'to/directory' }
        //])
    ],
    resolve: {
        extensions: ['.js', '.jsx', '.ts', '.tsx']
    },
};

module.exports = config;