import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import {Route} from 'react-router-dom';
import Auth from "./components/Auth/Auth";
import Alert from "./components/Alert/Alert";

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h1 className="App-title">Welcome to React</h1>
        </header>
          <Route path="/auth" component = {Auth} />
          <Route path="/alert" component = {Alert} />
      </div>
    );
  }
}

export default App;
