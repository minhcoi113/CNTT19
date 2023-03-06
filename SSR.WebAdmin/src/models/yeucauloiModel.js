const toJson = (item) => {
    return {
        id : item.id,
        Title : item.Title ,
        StepId:item.StepId,
        ProjectId:item.ProjectId,
        Label: item.Label,
        DueDate: item.DueDate,
        Donvi: item.Donvi,
        Assignee: item.Assignee,
        Description: item.Description,
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        Title : item.Title ,
        StepId:item.StepId,
        ProjectId:item.ProjectId,
        Label: item.Label,
        DueDate: item.DueDate,
        Donvi: item.Donvi,
        Assignee: item.Assignee,
        Description: item.Description,
    }
}

const baseJson = () => {
    return {
        id: null,
        Title : null ,
        StepId: "Open",
        ProjectId: null,
        Label: null,
        DueDate: null,
        Donvi: null,
        Assignee: null,
        Description: null,
    }
}



const toListModel = (items) =>{
    if(items.length > 0){
        let data = [];
        items.map((value, index) =>{
            data.push(fromJson(value));
        })
        return data??[];
    }
    return [];
}

export const yeucauloiModel = {
    toJson, fromJson, baseJson, toListModel
}
