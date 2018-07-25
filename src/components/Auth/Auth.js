import React, {Component} from "react";
import axios from "axios";

class Auth extends Component{

    state = {
        login: '',
        password: ''
    }
        loginHandler = (event) =>{
            console.log("loginHandler", event)
            this.setState({login: event.target.value})
        }

        passwodHandler = (event) => {
            console.log("passwodHandler", event);
            this.setState({ password: event.target.value });
        }

        submitHandler = () => {
            console.log(this.state);
            axios.post("api/auth/login", {login: this.state.login, password: this.state.password})
                .then(respone => {
                    console.log("res", respone)
                    if (respone.data.success === true) {
                        console.log("success logins")
                        this.props.history.push("/")
                    }
                }).catch(error => console.log("Error", error))
        }
    render(){
        return(
            <div>
                <p>Auth.Component</p>
                <input  onChange={this.loginHandler} value={this.state.login} />
                <input onChange={this.passwodHandler} value={this.state.password} />
                <button onClick={this.submitHandler}> Sing In</button>
            </div>)
    }
}

export default Auth;
