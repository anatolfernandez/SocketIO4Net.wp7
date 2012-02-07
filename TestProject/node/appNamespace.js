// SocketIO4Net - nodejs SocketIO Server
// version: v0.5.17
// node.exe v0.6.7

// require command-line options to start this application
var argv = require('optimist')
    .usage('Usage: $0 -host [localhost] -port [3000]')
    .demand(['host', 'port'])
    .argv;

var express = require('express')
  , server = express.createServer()
  , socketio = require('socket.io')

// configure Express
server.configure(function () {
    server.use(express.bodyParser());
    server.use('/content', express.static(__dirname + '/content'));
    server.use('/scripts', express.static(__dirname + '/scripts'));
});

// start server listening at host:port
server.listen(argv.port, argv.host); // http listen on host:port e.g. http://localhost:3000/

// configure Socket.IO
var io = socketio.listen(server); // start socket.io
io.set('log level', 5);


console.log('');
console.log('Nodejs Version: ', process.version);
console.log('     Listening: http://', argv.host, ':', argv.port);
console.log('');

// ***************************************************************
//    WEB Handlers  
//    Express guid: http://expressjs.com/guide.html
// ***************************************************************
server.get('/', function (req, res) {
    res.sendfile(__dirname + '/appNamespace.html');
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
  });

 
  io.sockets.on('connection', function (socket) {
     console.log('general socket connection');

     socket.on('disconnect', function () {
         io.sockets.emit('user disconnected');
     });
  });

  