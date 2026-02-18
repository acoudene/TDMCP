const express = require('express');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = 9999;
const DATA_FILE = path.join(__dirname, 'patients-data.json');

let patients = [];
let nextId = 11;

// Load initial data
try {
  const data = fs.readFileSync(DATA_FILE, 'utf8');
  patients = JSON.parse(data);
  console.log('Loaded', patients.length, 'patients from file');
} catch (error) {
  console.log('No initial data file found, starting with empty database');
  patients = [];
}

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// API Routes
app.get('/api/patients', (req, res) => {
  res.json(patients);
});

app.get('/api/patients/:id', (req, res) => {
  const patient = patients.find(p => p._id === req.params.id);
  if (!patient) {
    return res.status(404).json({ error: 'Patient not found' });
  }
  res.json(patient);
});

app.post('/api/patients', (req, res) => {
  const newPatient = {
    _id: String(nextId++),
    ...req.body
  };
  patients.push(newPatient);
  saveData();
  res.status(201).json(newPatient);
});

app.put('/api/patients/:id', (req, res) => {
  const index = patients.findIndex(p => p._id === req.params.id);
  if (index === -1) {
    return res.status(404).json({ error: 'Patient not found' });
  }
  const { _id, ...updateData } = req.body;
  patients[index] = { _id: req.params.id, ...updateData };
  saveData();
  res.json(patients[index]);
});

app.delete('/api/patients/:id', (req, res) => {
  const index = patients.findIndex(p => p._id === req.params.id);
  if (index === -1) {
    return res.status(404).json({ error: 'Patient not found' });
  }
  patients.splice(index, 1);
  saveData();
  res.json({ message: 'Patient deleted successfully' });
});

function saveData() {
  try {
    fs.writeFileSync(DATA_FILE, JSON.stringify(patients, null, 2));
  } catch (error) {
    console.error('Error saving data:', error);
  }
}

app.get('/', (req, res) => {
  const html = fs.readFileSync(__dirname + '/index.html', 'utf8');
  res.send(html);
});

app.listen(PORT, '0.0.0.0', () => {
  console.log('==============================================');
  console.log('  Server running on http://0.0.0.0:' + PORT);
  console.log('  Patients loaded:', patients.length);
  console.log('  CRUD Application ready!');
  console.log('==============================================');
});
