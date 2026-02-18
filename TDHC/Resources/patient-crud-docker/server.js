const express = require('express');
const { MongoClient, ObjectId } = require('mongodb');
const fs = require('fs');

const app = express();
const PORT = 9999;
const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://host.docker.internal:27017';
const DB_NAME = 'healthcare';
const COLLECTION_NAME = 'patients';

let db, collection;

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

async function connectDB() {
  try {
    const client = await MongoClient.connect(MONGODB_URI);
    db = client.db(DB_NAME);
    collection = db.collection(COLLECTION_NAME);
    console.log('Connected to MongoDB:', DB_NAME);
  } catch (error) {
    console.error('MongoDB connection error:', error);
    process.exit(1);
  }
}

app.get('/api/patients', async (req, res) => {
  try {
    const patients = await collection.find({}).toArray();
    res.json(patients);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.get('/api/patients/:id', async (req, res) => {
  try {
    const patient = await collection.findOne({ _id: new ObjectId(req.params.id) });
    if (!patient) return res.status(404).json({ error: 'Patient not found' });
    res.json(patient);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.post('/api/patients', async (req, res) => {
  try {
    const result = await collection.insertOne(req.body);
    const newPatient = await collection.findOne({ _id: result.insertedId });
    res.status(201).json(newPatient);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.put('/api/patients/:id', async (req, res) => {
  try {
    const { _id, ...updateData } = req.body;
    const result = await collection.findOneAndUpdate(
      { _id: new ObjectId(req.params.id) },
      { $set: updateData },
      { returnDocument: 'after' }
    );
    if (!result.value) return res.status(404).json({ error: 'Patient not found' });
    res.json(result.value);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.delete('/api/patients/:id', async (req, res) => {
  try {
    const result = await collection.deleteOne({ _id: new ObjectId(req.params.id) });
    if (result.deletedCount === 0) return res.status(404).json({ error: 'Patient not found' });
    res.json({ message: 'Patient deleted successfully' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.get('/', (req, res) => {
  const html = fs.readFileSync(__dirname + '/index.html', 'utf8');
  res.send(html);
});

connectDB().then(() => {
  app.listen(PORT, '0.0.0.0', () => {
    console.log('Server running on http://0.0.0.0:' + PORT);
    console.log('Database:', DB_NAME);
    console.log('Collection:', COLLECTION_NAME);
    console.log('CRUD Application ready!');
  });
});
