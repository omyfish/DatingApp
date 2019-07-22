import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoute: Routes = [
    { path: '', component: HomeComponent},
    { path: '', children: [
        { path: 'members', component: MemberListComponent },
        { path: 'messages', component: MessagesComponent },
        { path: 'lists', component: ListsComponent },
    ],
    runGuardsAndResolvers: 'always',
    canActivate : [AuthGuard]},
    { path: '**', redirectTo: 'home', pathMatch: 'full'},
];

