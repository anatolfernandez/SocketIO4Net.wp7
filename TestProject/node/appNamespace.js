// SocketIO4Net - nodejs SocketIO Server
// version: v0.7.5
// node.exe v0.8.9

// require command-line options to start this application
var argv = require('optimist')
    .usage('Usage: $0 -host [localhost] -port [3000]')
    .demand(['host', 'port'])
    .argv;

// require command-line options to start this application
var http = require('http'),
    express = require('express'),
    app = express(),
    socketio = require('socket.io');

// configure Express
app.configure(function () {
    app.use(express.bodyParser());
    app.use('/content', express.static(__dirname + '/content'));
    app.use('/scripts', express.static(__dirname + '/scripts'));
});

// start server listening at host:port
var server = http.createServer(app).listen(argv.port, argv.host);

// configure Socket.IO
var io = socketio.listen(server); // start socket.io

io.configure(function () {
    io.set('log level', 4),
    io.set('authorization', function (handshakeData, callback) {
        // auth simulation routine
        console.dir(handshakeData);
        callback(null, true); // error first callback style
    });
});

console.log('');
console.log('Nodejs Version: ', process.version);
console.log('     Listening: http://', argv.host, ':', argv.port);
console.log('');

// ***************************************************************
//    WEB Handlers  
//    Express guid: http://expressjs.com/guide.html
// ***************************************************************
app.get('/', function (req, res) {
    res.sendfile(__dirname + '/appNamespace.html');
});

io.sockets.on('connection', function (socket) {
     console.log('general socket connection');

     socket.on('disconnect', function () {
         io.sockets.emit('user disconnected');
     });
  });
  
// ***************************************************************
//    Socket.IO Client Handlers - with namespace
// ***************************************************************
// sample code from http://socket.io/#how-to-use - Restricting yourself to a namespace
var chat = io
  .of('/chat')
  .on('connection', function (socket) {
      console.log('client connected to [chat] namespace');
      /*
      socket.emit('a message', {
          that: 'only'
      , '/chat': 'will get'
      });
      chat.emit('a message', {
          everyone: 'in'
      , '/chat': 'will get'
      });
      */
  });

var news = io
.of('/news')
.on('connection', function (socket) {
    console.log('client connected to [news] namespace');
    // socket.emit('item', { news: 'item' });

    socket.on('disconnect', function () {
        console.log('client disconnected from [news] namespace');
    });
    socket.on('killme', function () {
        socket.disconnect();
    });
})
.authorization(function (handshakeData, callback) {
    console.dir(handshakeData);
    handshakeData.foo = 'baz';
    callback(null, true);
});

 
  

  