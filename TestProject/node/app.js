var app = require('express').createServer()
  , io = require('socket.io').listen(app);
io.set('log level',5);
app.listen(8080);

app.get('/', function (req, res) {
  res.sendfile(__dirname + '/index.html');
});

//io.set('browser client minification',true);
//io.set('browser client gzip',true);

io.sockets.on('connection', function (socket) {
  socket.emit('news', { hello: 'world' });
  
  socket.on('my other event', function (data) {
    console.log(data);
  });
  
  socket.on('messageAck', function (data, fn) {
    console.log('messageAck: ' + data);
	console.log(fn);
	fn({ hello: 'world' });
  });
});