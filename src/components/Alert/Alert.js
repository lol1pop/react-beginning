import React, {Component} from "react";
import axios from "axios";

class Alert extends Component{

    state={
        sendType:'',
        sendTitle:'',
        sendContents:'',
        sendUsers: [],
        sendGroups: []
    }


    render(){

        return(
            <div>
            <input  />
            <input  />
            <input  />
            <button >get alerts</button>
        </div>
        )
    }
}
export default Alert;